#!/software/anaconda3/bin

""" 

This code is based on the (dry) EBM described in Wagner, T.J. and I. Eisenman, 2015: 
How Climate Model Complexity Influences Sea Ice Stability. J. Climate, 28, 
3998–4014, https://doi.org/10.1175/JCLI-D-14-00654.1
See additional documentation at http://eisenman.ucsd.edu/code.html 

"""

import os
# import sys
# import xarray as xr
import numpy as np
import matplotlib.pyplot as plt
# import cartopy.crs as ccrs
from scipy.integrate import odeint
from scipy.interpolate import interp1d
from matplotlib.backends.backend_pdf import PdfPages
import time as datetime
start_time = datetime.time()

# Set up model run
n = 24
nt = 1000
dur = 30
dt = 1./nt
if dur < 30:
    raise Exception('Run duration must be at least 30 years')
# if n < 48:
#     raise Exception('Grid resolution must be at least 48 latitudes')

# Spatial Grid
dx = 1./n  # grid box width
x = np.arange(dx/2, 1, dx)  # native grid
lat = np.rad2deg(np.arcsin(x))
xb = np.arange(dx, 1, dx)


def model(F=0.):

    D = 0.6  # diffusivity for heat transport (W m^-2 K^-1)
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

    print(f'The diffusivity for heat transport is {D} W/m2/K')
    print(f'The radiative forcing is {F} W/m2')

    # Diffusion Operator (WE15, Appendix A)
    lam = D/dx**2*(1-xb**2)
    L1 = np.append(0, -lam)
    L2 = np.append(-lam, 0)
    L3 = -L1-L2
    diffop = -np.diag(L3[:-1] if n == 3 or n == 6 or n == 24 else L3) - \
        np.diag(L2[:n-1], 1) - np.diag(L1[1:n], -1)

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
    E100 = np.zeros((n, dur*100))
    T100 = np.zeros((n, dur*100))
    p = 0

    # Initial conditions
    T = 7.5+20*(1-2*x**2)
    Tg = T
    E = cw*T

    # Integration (see WE15_NumericIntegration.pdf)
    # Loop over Years
    for years in range(dur):
        # Loop within One Year
        for i in range(nt):
            # store 100 timesteps per year
            if i % (nt/100) == 0:
                E100[:, p] = E
                T100[:, p] = T
                p += 1
            # forcing
            alpha = aw*(E > 0) + ai*(E < 0)  # WE15, eq.4
            C = alpha*S[i, :] + cg_tau*Tg - A + F
            # surface temperature
            T0 = C/(M-kLf/E)  # WE15, eq.A3
            T = E/cw*(E >= 0)+T0*(E < 0)*(T0 < 0)  # WE15, eq.9
            # Forward Euler on E
            E = E+dt*(C-M*T+Fb)  # WE15, eq.A2
            # Implicit Euler on Tg
            Tg = np.linalg.solve(kappa-np.diag(dc/(M-kLf/E)*(T0 < 0)*(E < 0)),
                                 Tg + (dt_tau*(E/cw*(E >= 0)+(ai*S[i, :]-A+F)/(M-kLf/E)*(T0 < 0)*(E < 0))))

#    print('year %d complete' %(years))

    # output only converged, final year
    tfin = np.linspace(0, 1, 100)
    Efin = E100[:, -100:]
    Tfin = T100[:, -100:]
    print('Global mean annual mean surface temperature {:.2f} '.format(
        np.mean(Tfin, axis=(0, 1))))

    return tfin, Efin, Tfin


def main():

    # Integrate EBM
    tfin, Efin, Tfin = model(F=0.)

    print("--- %s seconds ---" % (datetime.time() - start_time))

    os.chdir(os.path.dirname(__file__))
    # Annual-mean hydrological cycle
    p_e = np.load("MERRA_P_E.dat")
    # MERRA has uniform grid-spacing
    lat_p_e = np.linspace(-90., 90., len(p_e))
    f = interp1d(lat_p_e[lat_p_e > 0], p_e[lat_p_e > 0])  # northern hemi only
    np_e = f(lat)
    alpha = 0.07  # assume precip increases by 7% per degree warming
    # need local, annual-mean temp from control (F=0) simulation to scale precip
    tfin_ctl, Efin_ctl, Tfin_ctl = model(F=0.)
    dT = np.mean(Tfin, axis=1) - np.mean(Tfin_ctl, axis=1)
    dp_e = dT * alpha * np_e  # change in net precip compared to control
    net_precip = np_e + dp_e  # P-E (units mm/day)

    fig0 = plt.figure()
    plt.plot(x, net_precip)
    plt.xlabel('x')
    plt.ylabel('P-E [mm/day]')
    plt.grid()

    # WE15, Figure 2: Default Steady State Climatology
    winter = 26  # time of coldest <T>
    summer = 76  # time of warmest <T>
    # compute seasonal ice edge
    xi = np.zeros(100)
    # if isempty(find(E<0,1))==0:
    for j in range(0, len(tfin)):
        E = Efin[:, j]
        if any(E < 0):
            ice = np.where(E < 0)[0]
            xi[j] = x[ice[0]]
        else:
            xi[j] = max(x)

    fig = plt.figure(figsize=(10, 5))

    # plot enthalpy (Fig 2a)
    plt.subplot(141)
    clevsE = np.append(np.arange(-40, 20, 20), np.arange(50, 350, 50))
    plt.contourf(tfin, x, Efin, clevsE)
    plt.colorbar()
    # plot ice edge on E
    plt.plot(tfin, xi, 'k')
    plt.xlabel('t (final year)')
    plt.ylabel('x')
    plt.ylim(0, 1)
    plt.title(r'E (J m$^{-2}$)')

    # plot temperature (Fig 2b)
    plt.subplot(142)
    clevsT = np.arange(-30.001, 35., 5.)
    plt.contourf(tfin, x, Tfin, clevsT)
    plt.colorbar()
    # plot ice edge on T
    plt.plot(tfin, xi, 'k')
    # plot T=0 contour (the region between ice edge and T=0 contour is the
    # region of summer ice surface melt)
    plt.contour(tfin, x, Tfin, [-0.001], colors='r', linestyles='-')
    plt.xlabel('t (final year)')
    plt.ylabel('x')
    plt.ylim(0, 1)
    plt.title(r'T ($^\circ$C)')

    # plot the ice thickness (Fig 2c)
    Lf = 9.5  # sea ice latent heat of fusion (W yr m^-3)
    plt.subplot(1, 4, 3)
    clevsh = np.arange(0.00001, 5.5, .5)
    icethick = -Efin/Lf*(Efin < 0)
    plt.contourf(tfin, x, icethick, clevsh)
    plt.colorbar()
    # plot ice edge on h
    plt.contour(tfin, x, icethick, [0], colors='k')
    plt.plot([tfin[winter], tfin[winter]], [0, max(x)], 'k')
    plt.plot([tfin[summer], tfin[summer]], [0, max(x)], 'k--')
    plt.xlabel('t (final year)')
    plt.ylabel('x')
    plt.ylim(0, 1)
    plt.title('h (m)')

    # plot temperature profiles (Fig 2d)
    plt.subplot(444)
    Summer, = plt.plot(x, Tfin[:, summer], 'k--', label='summer')
    Winter, = plt.plot(x, Tfin[:, winter], 'k', label='winter')
    plt.plot([0, 1], [0, 0], 'k')
    plt.xlabel('x')
    plt.ylabel(r'T ($^\circ$C)')
    plt.legend(handles=[Summer, Winter], loc=0, fontsize=8)

    # plot ice thickness profiles (Fig 2e)
    plt.subplot(448)
    plt.plot(x, icethick[:, summer], 'k--')
    plt.plot(x, icethick[:, winter], 'k')
    plt.plot([0, 1], [0, 0], 'k')
    plt.xlim([0.7, 1])
    plt.xlabel('x')
    plt.ylabel('h (m)')

    # plot seasonal thickness cycle at pole (Fig 2f)
    plt.subplot(4, 4, 12)
    plt.plot(tfin, icethick[-1, :], 'k')
    plt.xlabel('t (final year)')
    plt.ylabel(r'h$_p$ (m)')

    # plot ice edge seasonal cycle (Fig 2g)
    plt.subplot(4, 4, 16)
    xideg = np.degrees(np.arcsin(xi))
    plt.plot(tfin, xideg, 'k-')
    plt.ylim([0, 90])
    plt.xlabel('t (final year)')
    plt.ylabel(r'$\theta_i$ (deg)')

    plt.tight_layout()
    plt.show()


if __name__ == '__main__':
    main()
