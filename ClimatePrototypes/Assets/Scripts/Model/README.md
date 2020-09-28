# **EBM**

## **Background**
The energy balance model (EBM) is described in the following paper: Wagner, T.J. and I. Eisenman, 2015: How Climate Model Complexity Influences Sea Ice Stability. J. Climate, 28, 3998–4014, https://doi.org/10.1175/JCLI-D-14-00654.1. Original code and documentation for dry model are available at http://eisenman.ucsd.edu/code.html.

The time evolution of surface enthalpy E(t, x) is determined at each latitude by the net energy flux into the atmospheric column and surface below by seasonally varying solar radition, outgoing longwave radiation, diffusive atmospheric heat transport, and ocean heat flux. The fraction of incident solar radiation that is absorbed is called the planetary, or top-of-atmosphere (TOA), coalbedo (i.e., one minus albedo). It depends on factors including the solar zenith angle, clouds, and the presence of reflective ice at the surface. The model is forced by a uniform value, F, which can be interpreted as a change in atmospheric carbon dioxide. Additionally, the EBM includes a representation of thermodynamic sea ice growth and melt, which affects surface temperature.

## **Initialisation**

Call the `EBM.Calc()` method with no parameters to start the model with default values and initialise the temp, energy, and precip variables

## **Methods**

- `EBM.Calc()`
  - wrapper for the main integration methods that also updates public variables
  - also returns public variables as arrays
- `EBM.Integrate()`
  - internal loop function that calculates and stabilises the model based on initial conditions
- `EBM.CalcPrecip()`
  - takes resulting temperature and calculates precipitation

## **Fetching Values**

Three main values can be read from EBM

- `EBM.temp`<br>
  `Vector<double>` of Temperature of each region
- `EBM.energy`<br>
  `Vector<double>` of Energy of each region
- `EBM.precip`<br>
  `Vector<double>` of Precipitation of each region

`Vector<T>` can be accessed like any array, documentation available [here](https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra.Double/Vector.htm) and [here](https://numerics.mathdotnet.com/api/MathNet.Numerics.LinearAlgebra/VectorBuilder%601.htm)(constructor).

## **Model Parameters**

In general the default model parameters should *not* be adjusted:
  
| Variable | Data Type | Description                                                            | Default |
| -------- | --------- | ---------------------------------------------------------------------- | ------- |
| `EBM.Fb` | `double`  | Heat Flux from deep ocean to base of sea ice or ocean mixed layer      | `4`     |
| `EBM.S0` | `double`  | Solar radiation at equator, affected by orbital parameters             | `420`   |
| `EBM.S1` | `double`  | Solar radiation seasonal dependence, affected by orbital parameters    | `338`   |
| `EBM.S2` | `double`  | Solar radiation spatial dependence, affected by orbital parameters     | `240`   |
| `EBM.cw` | `double`  | Ocean Heat Capacity                                                    | `9.8`   |
| `EBM.D`  | `double`  | Heat Transport Diffusivity, adjusts sea ice boundaries                 | `0.6`   |
| `EBM.A`  | `double`  | Outgoing Longwave Radiation at T=0 deg. C, controls base climate state | `193`   |
| `EBM.B`  | `double`  | Longwave feedback parameter, affects amount of warming                 | `2.1`   |
| `EBM.a0` | `double`  | Ice-free coalbedo at equator                                           | `0.7`   |
| `EBM.a2` | `double`  | Ice-free coalbedo spatial dependence                                   | `0.1`   |
| `EBM.ai` | `double`  | Coalbedo where there is sea ice                                        | `0.4`   |

These values may be adjusted before runtime:

| Variable | Data Type | Description                                            | Default |
| -------- | --------- | -------------------------------------------------------| ------- |
| `EBM.F`  | `double`  | Radiative Forcing, represents change in CO2            | `0`     |
| `EBM.n`  | `int`     | Latitudinal bands, number of data points to calculate  | `24`    |

You can just assign new values based on game conditions and rerun the model with `EBM.calc()`<br>
ie. `EBM.F = 4; // assigns new forcing value`

`EBM.Calc()` takes the following parameters:<br>

| Param       | Data Type        | Description                      | Default         |
| ----------- | ---------------- | -------------------------------- | --------------- |
| `input`     | `Vector<double>` | Starting temperature             | `null`*         |
| `years`     | `int`            | number of years to run the model | `EBM.dur = 30`  |
| `timesteps` | `int`            | number of iterations per year    | `EBM.nt = 1000` |

\* Assigned internally to `7.5 + 20 * (1-x**2)`<br>
After adjusting parameters and rerun-ing the model with `EBM.Calc()`, temp, energy, and precip will be updated with new values.
