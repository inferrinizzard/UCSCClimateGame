# **EBM**

## **Background**
The energy balance model (EBM) is a moist version of the EBM described in Wagner and Eisenman (2015). The time evolution of E(t, x) is determined at each latitude by the net energy flux into the atmospheric column and surface below:

![equation](https://latex.codecogs.com/gif.latex?%5Cfrac%7B%5Cpartial%20E%7D%7B%5Cpartial%20t%7D%20%3D%20aS%20-%20L%20&plus;%20D%5Cnabla%5E2%20h%20&plus;%20F%20&plus;%20F_b)

Solar radiation S(t,x) depends on season and latitude. The fraction of incident solar radiation that is absorbed is called the planetary co-albedo, a. It depends on factors including the solar zenith angle, clouds, and the presence of reflective ice.

Outgoing longwave radiation is represented as a linear function of surface temperature

![equation](https://latex.codecogs.com/gif.latex?%5BA&plus;B%28T-T_m%29%5D)
where A and B are empirical parameters and T_m the melting point.

Atmospheric energy transport is governed by the diffusion of moist static energy. On a spherical Earth, meridional diffusion takes the form 

![equation](https://latex.codecogs.com/gif.latex?D%5Cnabla%5E2%20h%20%3D%20D%5Cfrac%7B%5Cpartial%7D%7B%5Cpartial%20x%7D%5Cbigg%5B%281-x%5E2%29%5Cfrac%7B%5Cpartial%20h%7D%7B%5Cpartial%20x%7D%5Cbigg%5D)

D is the diffusivity, and moist static energy is defined in units of temperature as 
![equation](https://latex.codecogs.com/gif.latex?%24h%3DT&plus;L%5Cmathcal%7BH%7Dc_p%5E%7B-1%7Dq_s%24)
, assuming constant relative humidity H of 80% (Merlis and Henry, 2018). L is latent heat of vaporization, cp is specific heat of air at constant pressure, qs is saturation specific humidity.

Radiative forcing, F, and heat flux into the model domain from the ocean below, Fb, are global parameters.

## **Initialisation**

Call the `EBM.calc()` method with no parameters to start the model with default values and initialise the temp, energy, and precip variables

## **Methods**

- `EBM.calc()`
  - wrapper for the main integration methods that also updates public variables
- `EBM.integrate()`
  - the main loop function that calculates and stabilises the model based on initial conditions
- `EBM.calcPrecip()`
  - takes resulting temperature and calculates precipitation

## **Fetching Values**

Three main values can be read from EBM

- `EBM.temp`<br>
  `Vector<double>` of Temperature of each region
- `EBM.energy`<br>
  `Vector<double>` of Energy of each region
- `EBM.precip`<br>
  `Vector<double>` of Precipitation of each region

`Vector<T>` can be accessed like any array, documentation available [here](https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Vector.htm).

## **Changing Values**

The following values may be adjusted during runtime:

- ~~`EBM.A` \<double> <br> 
  ~~(OLR, controls how much energy is released)
- `EBM.F` \<double> <br>
  (Radiative Forcing, affected by amount of CO2)<br>
  
| Variable | Data Type | Description                                                         | Default |
| -------- | --------- | ------------------------------------------------------------------- | ------- |
| `EBM.F`  | `double`  | Radiative Forcing, affected by atmospheric amount of CO2            | `0`     |
| `EBM.Fb` | `double`  | Heat Flux from deep ocean to base of sea ice or ocean mixed layer   | `4`     |
| `EBM.S0` | `double`  | Solar radiation at equator, affected by orbital parameters          | `420`   |
| `EBM.S1` | `double`  | Solar radiation seasonal dependence, affected by orbital parameters | `338`   |

These values are to be adjusted before runtime:

| Variable | Data Type | Description                                                    | Default | Alt Value |
| -------- | --------- | -------------------------------------------------------------- | ------- | --------- |
| `EBM.n`  | `int`     | Latitudinal bands, number of data points to calculate          | `12`    | `24`      |
| `EBM.cw` | `double`  | Ocean Heat Capacity, controls the speed of model stabilisation | `9.8`   | `0.98`    |
| `EBM.D`  | `double`  | Heat Transport Diffusivity, adjusts sea ice boundaries         | `0.5`   | `0.45`    |

You can just assign new values based on game conditions and rerun the model with `EBM.calc()`<br>
ie. `EBM.F = 4; // assigns new forcing value`

`EBM.calc()` takes the following parameters:<br>

| Param       | Data Type        | Description                      | Default         |
| ----------- | ---------------- | -------------------------------- | --------------- |
| `input`     | `Vector<double>` | Starting temperature             | `null`          |
| `years`     | `int`            | number of years to run the model | `EBM.dur = 30`  |
| `timesteps` | `int`            | number of iterations per year   | `EBM.nt = 1000` |

After adjusting parameters and rerun-ing the model with `EBM.calc()`, temp, energy, and precip will be updated with new values.

### References
Wagner, T.J. and I. Eisenman, 2015: How Climate Model Complexity Influences Sea Ice Stability. J. Climate, 28, 3998–4014, https://doi.org/10.1175/JCLI-D-14-00654.1. Original code and documentation for dry model available at http://eisenman.ucsd.edu/code.html.
Merlis, T.M. and M. Henry, 2018: Simple Estimates of Polar Amplification in Moist Diffusive Energy Balance Models. J. Climate, 31, 5811–5824, https://doi.org/10.1175/JCLI-D-17-0578.1 

