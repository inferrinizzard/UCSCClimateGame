# Till Wagner's python version:
#
# Reference: "How Model Complexity Influences Sea Ice Stability",
# T.J.W. Wagner & I. Eisenman, J Clim (2015)
#
# WE15_EBM_fast.m:
# This code describes the EBM as discussed in Sec. 2b of the article above,
# hereafter WE15. Here we use central difference spatial integration and
# Implicit Euler time stepping.
#
# The code WE15_EBM_simple.m, on the other hand, uses a simpler formulation
# of the diffusion operator and time stepping with Matlab's ode45.
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
#
# -------------------------------------------------------------------------
import numpy as np
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
# -------------------------------------------------------------------------
n = 3  # grid resolution (number of points between equator and pole)
nt = 5
dur = 100
dt = 1/nt
# Spatial Grid ---------------------------------------------------------
dx = 1.0/n  # grid box width
x = np.arange(dx/2, 1+dx/2, dx)  # native grid
xb = np.arange(dx, 1, dx)
# Diffusion Operator (WE15, Appendix A) -----------------------------------
lam = D/dx**2*(1-xb**2)
L1 = np.append(0, -lam)
L2 = np.append(-lam, 0)
L3 = -L1-L2
d3 = np.diag(L3[:-1] if n == 3 or n == 6 else L3)
d2 = np.diag(L2[:n-1], 1)
d1 = np.diag(L1[1:n], -1)

diffop = -d3-d2-d1
# diffop = - np.diag(L3) - np.diag(L2[:n-1], 1) - np.diag(L1[1:n], -1)

S = S0-S2*x**2  # insolation [WE15 eq. (3) with S_1 = 0]
aw = a0-a2*x**2  # open water albedo

T = 10*np.ones(x.shape)  # initial condition (constant temp. 10C everywhere)
allT = np.zeros([dur*nt, n])
t = np.linspace(0, dur, dur*nt)

I = np.identity(n)
invMat = np.linalg.inv(I+dt/cw*(B*I-diffop))
# print(I+dt/cw*(B*I-diffop))

# integration over time using implicit difference and
# over x using central difference (through diffop)

for i in range(0, int(dur*nt)):
    a = aw*(T > 0)+ai*(T < 0)  # WE15, eq.4
    C = a*S-A+F
    print(np.dot(diffop , T))
    print((diffop @ T))
    # print(invMat)
    # print(C)
    T0 = T+dt/cw*C
    # print(T0)
    # h0 = h + dt/cw*C
    # Governing equation [cf. WE15, eq. (2)]:
    # T(n+1) = T(n) + dt*(dT(n+1)/dt), with c_w*dT/dt=(C-B*T+diffop*T)
    # -> T(n+1) = T(n) + dt/cw*[C-B*T(n+1)+diff_op*T(n+1)]
    # -> T(n+1) = inv[1+dt/cw*(1+B-diff_op)]*(T(n)+dt/cw*C)
    T = np.dot(invMat, T0)
    allT[i, :] = T

print(allT)

fig = plt.figure(1)
fig.suptitle('EBM_fast_WE15')
plt.subplot(121)
plt.plot(t, allT)
plt.xlabel('t (years)')
plt.ylabel('T (in $^\circ$C)')
plt.subplot(122)
plt.plot(x, T)
plt.xlabel('x')
plt.show()
