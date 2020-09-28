#!/software/anaconda3/bin

""" 

This code is based on the (dry) EBM described in Wagner, T.J. and I. Eisenman, 2015: 
How Climate Model Complexity Influences Sea Ice Stability. J. Climate, 28, 
3998–4014, https://doi.org/10.1175/JCLI-D-14-00654.1
See additional documentation at http://eisenman.ucsd.edu/code.html 

Updates:

(1) We account for latent heat transport following Merlis, T.M. and M. Henry, 2018: 
Simple Estimates of Polar Amplification in Moist Diffusive Energy Balance Models. 
J. Climate, 31, 5811–5824, https://doi.org/10.1175/JCLI-D-17-0578.1 

(2) We incorporate a simple Hadley cell parameterization into the MEBM, which allows 
latent heat to travel upgradient even while total energy transport is downgradient, 
thus permitting a more realistic representation of the hydrologic cycle. Siler, N., 
G.H. Roe, and K.C. Armour, 2018: Insights into the Zonal-Mean Response of the 
Hydrologic Cycle to Global Warming from a Diffusive Energy Balance Model. 
J. Climate, 31, 7481–7493, https://doi.org/10.1175/JCLI-D-18-0081.1 


"""

import sys
import xarray as xr
import numpy as np
import matplotlib.pyplot as plt
import cartopy.crs as ccrs
from scipy.integrate import odeint
from matplotlib.backends.backend_pdf import PdfPages
import time as datetime
start_time = datetime.time()


def saturation_specific_humidity(temp, press):
    """
    We assume a single liquid-to-vapor phase transition with the parameter values 
    of the Clausius–Clapeyron (CC) relation given in O’Gorman and Schneider (2008) 
    to determine the saturation specific humidity qs(T).

    """

    es0 = 610.78  # saturation vapor pressure at t0 (Pa)
    t0 = 273.16
    Rv = 461.5
    Lv = 2.5E6
    ep = 0.622  # ratio of gas constants of dry air and water vapor
    temp = temp + 273.15  # convert to Kelvin
    es = es0 * np.exp(-(Lv/Rv) * ((1/temp)-(1/t0)))
    qs = ep * es / press

    return qs


def model(D=0.5, F=0):

    print(f'The diffusivity for heat transport is {D} W/m2/K')
    print(f'The radiative forcing is {F} W/m2')

    # D = 0.5 # diffusivity for heat transport (W m^-2 K^-1)
    S1 = 338  # insolation seasonal dependence (W m^-2)
    A = 193  # OLR when T = 0 (W m^-2)
    B = 2.1  # OLR temperature dependence (W m^-2 K^-1)
    cw = 9.8  # ocean mixed layer heat capacity (W yr m^-2 K^-1)
    S0 = 420  # insolation at equator (W m^-2)
    S2 = 240  # insolation spatial dependence (W m^-2)
    a0 = 0.7  # ice-free co-albedo at equator
    a2 = 0.1  # ice=free co-albedo spatial dependence
    ai = 0.4  # co-albedo where there is sea ice
    Fb = 4  # heat flux from ocean below (W m^-2)
    # F = 0 # radiative forcing (W m^-2)
    k = 2  # sea ice thermal conductivity (W m^-2 K^-1)
    Lf = 9.5  # sea ice latent heat of fusion (W yr m^-3)
    cg = 0.01*cw  # ghost layer heat capacity(W yr m^-2 K^-1)
    tau = 1e-5  # ghost layer coupling timescale (yr)
    Lv = 2.5E6  # latent heat of vaporization (J kg^-1)
    cp = 1004.6  # heat capacity of air at constant pressure (J kg^-1 K^-1)
    RH = 0.8  # relative humidity
    Ps = 1E5  # surface pressure (Pa)

    # The default run in WE15, Fig 2 uses the time-stepping parameters:
    # n=400 % # of evenly spaced latitudinal gridboxes (equator to pole)
    # nt=1e3 % # of timesteps per year (approx lower limit of stability)
    # dur=200 % # of years for the whole run
    # For a quicker computation, use the parameters:
    n = 50
    nt = 1000
    dur = 30
    dt = 1/float(nt)

    # Spatial Grid
    dx = 1.0/n  # grid box width
    x = np.arange(dx/2, 1+dx/2, dx)  # native grid
    xb = np.arange(dx, 1, dx)

    # Diffusion Operator (WE15, Appendix A)
    lam = D/dx**2*(1-xb**2)
    L1 = np.append(0, -lam)
    L2 = np.append(-lam, 0)
    L3 = -L1-L2
    diffop = - np.diag(L3) - np.diag(L2[:n-1], 1) - np.diag(L1[1:n], -1)

    # Definitions for implicit scheme on Tg
    cg_tau = cg/tau
    dt_tau = dt/tau
    dc = dt_tau*cg_tau
    kappa = (1+dt_tau)*np.identity(n)-dt*diffop/cg

    # Seasonal forcing (WE15 eq.3)
    ty = np.arange(dt/2, 1+dt/2, dt)
    S = (np.tile(S0-S2*x**2, [nt, 1]) -
         np.tile(S1*np.cos(2*np.pi*ty), [n, 1]).T*np.tile(x, [nt, 1]))

    # Further definitions
    M = B+cg_tau
    aw = a0-a2*x**2  # open water albedo
    kLf = k*Lf

    # Set up output arrays, saving 100 timesteps/year
    E100 = np.zeros([n, dur*100])
    T100 = np.zeros([n, dur*100])
    p = -1
    m = -1

    # Initial conditions
    T = 7.5+20*(1-2*x**2)
    Tg = T
    E = cw*T

    # Integration (see WE15_NumericIntegration.pdf)
    # Loop over Years
    for years in range(0, dur):
        # Loop within One Year
        for i in range(0, int(nt)):
            m = m+1
            # store 100 timesteps per year
            if (p+1)*10 == m:
                p = p+1
                E100[:, p] = E
                T100[:, p] = T
            # forcing
            alpha = aw*(E > 0) + ai*(E < 0)  # WE15, eq.4
            C = alpha*S[i, :] + cg_tau*Tg - A
            # surface temperature
            T0 = C/(M-kLf/E)  # WE15, eq.A3
            T = E/cw*(E >= 0)+T0*(E < 0)*(T0 < 0)  # WE15, eq.9
            # Forward Euler on E
            E = E+dt*(C-M*T+Fb+F)  # WE15, eq.A2
            # latent heat transport
            q = RH * saturation_specific_humidity(Tg, Ps)
            lht = dt * np.dot(diffop, Lv*q/cp)
            # Implicit Euler on Tg
            Tg = np.linalg.solve(kappa-np.diag(dc/(M-kLf/E)*(T0 < 0)*(E < 0)),
                                 Tg + lht + (dt_tau*(E/cw*(E >= 0)+(ai*S[i, :]-A)/(M-kLf/E)*(T0 < 0)*(E < 0))))
        print('year %d complete' % (years))

    # output only converged, final year
    tfin = np.linspace(0, 1, 100)
    Efin = E100[:, -100:]
    Tfin = T100[:, -100:]
    print('Global mean annual mean surface temperature {:.2f} '.format(
        np.mean(Tfin, axis=(0, 1))))

    # Plot timeseries
    plt.figure()
    plt.plot(np.mean(T100, axis=0))
    plt.gca().set_ylabel(r'T ($^\circ$C)')
    plt.gca().set_xlabel('time')
    plt.gca().set_title('Temperature', fontsize=10)

    return x, tfin, Efin, Tfin


def hydro_cycle(Tfin, D, x):

    Lv = 2.5E6  # latent heat of vaporization (J kg^-1)
    cp = 1004.6  # heat capacity of air at constant pressure (J kg^-1 K^-1)
    RH = 0.8  # relative humidity
    Ps = 1E5  # surface pressure (Pa)
    gms_scale = 2.  # 1.06 # ratio of MSE aloft to near surface, equatorial MSE
    sigma = 0.4  # 0.3 # characteristic width for gaussian weighting function

    # Compute hydrological cycle for final year
    # Calculate diffusive heat transport, latent and total
    x = np.reshape(x, (len(x), 1))
    qfin = RH * saturation_specific_humidity(Tfin, Ps)
    hfin = Tfin + Lv*qfin/cp  # in temperature units
    Fa = -D * (1-x**2) * np.gradient(hfin, axis=0, edge_order=2) / \
        np.gradient(x, axis=0, edge_order=2)
    Fl = -D * (1-x**2) * np.gradient(Lv*qfin/cp, axis=0,
                                     edge_order=2) / np.gradient(x, axis=0, edge_order=2)

    # Weighting function to partition transport into Hadley cell and eddy transports
    w = np.exp(-x**2 / sigma**2)
    F_hc = w*Fa
    F_eddy = (1-w)*Fa
    Fl_eddy = (1-w)*Fl

    # Calculate latent heat transport by Hadley cell
    hfin_eq = hfin[0, :]
    gms = hfin_eq * gms_scale - hfin  # gross moist stability in temperature units
    psi = F_hc / gms  # mass flux
    Fl_hc = -(Lv*qfin/cp) * psi

    # Calculate E-P as the convergence of latent heat transport
    Fltot = Fl_hc + Fl_eddy
    EminusP = np.gradient(Fltot, axis=0, edge_order=2) / \
        np.gradient(x, axis=0, edge_order=2)

    return EminusP


def main():

    D = 0.5

    # Integrate control EBM
    x, tfin, Efin, Tfin_ctl = model(D=D, F=0)
    # Calculate hydro cycle from last year
    EminusP_ctl = hydro_cycle(Tfin_ctl, D, x)

    # Perturbation runs
    forcing = 4
    x, tfin, Efin, Tfin_ptb = model(D=D, F=forcing)
    dT = np.mean(Tfin_ptb - Tfin_ctl, axis=1)
    # Calculate hydro cycle from last year
    EminusP_ptb = hydro_cycle(Tfin_ptb, D, x)
    dEminusP = np.mean(EminusP_ptb - EminusP_ctl, axis=1)

    # Plots
    fig, ax = plt.subplots(2, 2)
    ax[0, 0].plot(x, np.mean(Tfin_ctl, axis=1), label=f'F=0 W/m2')
    ax[0, 0].plot(x, np.mean(Tfin_ptb, axis=1), label=f'F={forcing} W/m2')
    ax[0, 0].set_xlim(0, 1)
    ax[0, 0].set_ylabel(r'T ($^\circ$C)')
    ax[0, 0].set_xlabel('latitude')
    ax[0, 0].legend()
    ax[0, 0].set_title('Average of final year', fontsize=10)

    ax[1, 0].plot(x, dT)
    ax[1, 0].set_xlim(0, 1)
    ax[1, 0].set_ylabel(r'$\Delta$T ($^\circ$C)')
    ax[1, 0].set_xlabel('latitude')
    ax[1, 0].set_xlabel('x')

    ax[0, 1].plot(x, np.mean(EminusP_ctl, axis=1), label=f'F=0 W/m2')
    ax[0, 1].plot(x, np.mean(EminusP_ptb, axis=1), label=f'F={forcing} W/m2')
    ax[0, 1].set_xlim(0, 1)
    ax[0, 1].set_ylabel(r'E-P (W m$^{-2}$)')
    ax[0, 1].set_xlabel('latitude')
    ax[0, 1].legend()
    ax[0, 1].set_title('Average of final year', fontsize=10)

    ax[1, 1].plot(x, dEminusP)
    ax[1, 1].set_xlim(0, 1)
    ax[1, 1].set_ylabel(r'$\Delta$ E-P (W m$^{-2}$)')
    ax[1, 1].set_xlabel('latitude')
    ax[1, 1].set_xlabel('x')

    fig.tight_layout()
    plt.show()


if __name__ == '__main__':
    main()
