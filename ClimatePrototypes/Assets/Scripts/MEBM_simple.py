# Modification of Wagner and Eisenman's simple EBM to account for moist
# processes, following Merlis and Henry (JCLI, 2018).
#
# Till Wagner's python version:
#
# Reference: "How Model Complexity Influences Sea Ice Stability",
# T.J.W. Wagner & I. Eisenman, J Clim (2015)
#
# WE15_EBM_simple.m:
# This code describes the EBM as discussed in Sec. 2b of the article above,
# hereafter WE15. Here we use central difference spatial integration and
# time stepping with MATLAB's ode45.
#
# The code WE15_EBM_fast.m, on the other hand, uses a faster, but more
# complicated formulation of the diffusion operator and Implicit Euler time
# stepping.
#
# Parameters are as described in WE15, table 1. Note that we do not include
# ocean heat flux convergence or a seasonal cylce in the forcing
# (equivalent to S_1 = F_b = 0 in WE15). This code uses an ice albedo when
# T<0 (WE15 instead uses the condition E<0, which is appropriate for the
# inclusion of a seasonal cycle in ice thickness). In this code, we define
# T = Ts - Tm, where Ts is the surface temperature and Tm the melting point
# (WE15, by contrast, defines T = Ts).
#
# Till Wagner & Ian Eisenman, Mar 15
# tjwagner@ucsd.edu or eisenman@ucsd.edu
# -------------------------------------------------------------------------
import numpy as np
from scipy.integrate import odeint
import matplotlib.pyplot as plt
# Model parameters (WE15, Table 1 and Section 2d) -------------------------
D = 0.6  # diffusivity for heat transport (W m^-2 K^-1)
A = 193  # OLR when T = 0 (W m^-2)
B = 2.1  # OLR temperature dependence (W m^-2 K^-1)
cw = 9.8  # ocean mixed layer heat capacity (W yr m^-2 K^-1)
S0 = 420  # insolation at equator (W m^-2)
S2 = 240  # insolation spatial dependence (W m^-2)
a0 = 0.7  # ice-free co-albedo at equator
a2 = 0.1  # ice=free co-albedo spatial dependence
ai = 0.4  # co-albedo where there is sea ice
F = 0  # radiative forcing (W m^-2)
Lv = 2.5E6  # latent heat of vaporization (J kg^-1)
cp = 1004.6  # heat capacity of air at constant pressure (J kg^-1 K^-1)
Rh = 0.8  # relative humidity
Ps = 1E5  # surface pressure (Pa)
# -------------------------------------------------------------------------
n = 3  # grid resolution (number of points between equator and pole)
x = np.linspace(0, 1, n)
dx = 1.0/(n-1)
S = S0-S2*x**2  # insolation [WE15 eq. (3) with S_1 = 0]
aw = a0-a2*x**2  # open water albedo


def saturation_specific_humidity(temp, press):
    """
    We assume a single liquid-to-vapor phase transition with the parameter values 
    of the Clausius–Clapeyron (CC) relation given in O’Gorman and Schneider (2008) 
    to determine the saturation specific humidity qs(T).

    """

    es0 = 610.78  # saturation vapor pressure at t0 (Pa)
    t0 = 273.16
    Rv = 461.5
    ep = 0.622  # ratio of gas constants of dry air and water vapor
    temp = temp + 273.15  # convert to Kelvin
    es = es0 * np.exp(-(Lv/Rv) * ((1/temp)-(1/t0)))
    qs = ep * es / press
    # print(es)
    # print(qs)
    return qs

# ODE with spatial finite differencing-------------------------------------


j = 0


def odefunc(T, t):
    # print(T)
    global j
    alpha = aw*(T > 0)+ai*(T < 0)
    C = alpha*S-A+F
    hdot = np.zeros(x.shape)
    qs = saturation_specific_humidity(T, Ps)
    # print(T)
    q = Rh * qs  # specific humidity
    h = T + (Lv * q) / cp  # moist static energy in units of temperature
    # print(h)
    # h = T  # moist static energy in units of temperature
    # solve c_wdT/dt = D(1-x^2)d^
    for i in range(1, n-1):
        hdot[i] = (D/dx**2)*(1-x[i]**2)*(h[i+1]-2*h[i]+h[i-1]) - \
            (D*x[i]/dx)*(h[i+1]-h[i-1])
    # solve c_w dT/dt = D (1-x^2) d^2 T/dx^2 - 2 x D dT/dx + C - B T [cf. WE15 eq. (2)]
    # use central difference
    hdot[0] = D*2*(h[1]-h[0])/dx**2
    hdot[-1] = -D*2*x[-1]*(h[-1]-h[-2])/dx
    f = (hdot+C-B*T)/cw
    # print(hdot)
    print(str(T)+" "+str(f) + " " + str(j))
    # print(B)
    # print(C)
    # print(cw)
    j = j+1
    # if(j == 5):
    #     exit()
    return f


T0 = 10*np.ones(x.shape)  # initial condition (constant temp. 10C everywhere)
time = np.linspace(0.0, 30.0, 5)  # time span in years
print(time)
sol = odeint(odefunc, T0, time)  # solve
print(sol)

exit()
fig = plt.figure(1)
fig.suptitle('MEBM_simple')
plt.subplot(121)
plt.plot(time, sol)
plt.xlabel('t (years)')
plt.ylabel('T (in $^\circ$C)')
plt.subplot(122)
plt.plot(x, sol[-1, :])
plt.xlabel('x')
plt.show()
