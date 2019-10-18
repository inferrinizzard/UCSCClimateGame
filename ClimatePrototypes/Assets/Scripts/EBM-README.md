# **EBM**

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

- `EBM.A` \<double> <br>
  (OLR, controls how much energy is released)
- `EBM.F` \<double> <br>
  (Radiative Forcing, affected by amount of CO)<br>

These values are to be adjusted before runtime:

- `EBM.n` \<int> <br>
  (Latitudinal bands, number of data points to calculate)
- `EBM.cw` \<double> <br>
  (Ocean Heat Capacity, controls the speed of model stabilisation)<br>
- `EBM.D` \<double> <br>
  (Heat Transport Diffusivity, adjusts sea ice boundaries)

You can just assign new values based on game conditions and rerun the model with `EBM.calc()`<br>
ie. `EBM.F = 4; // assigns new forcing value`

`EBM.calc()` takes the following parameters:<br>

| Param | Data Type | Description | Default |
| --- | --- | --- | --- |
| `input`     | `Vector<double>` | Starting temperature             | `null`          |
| `years`     | `int`            | number of years to run the model | `EBM.dur = 30`  |
| `timesteps` | `int`            | number of interations per year   | `EBM.nt = 1000` |

After adjusting parameters and rerun-ing the model with `EBM.calc()`, temp, energy, and precip will be updated with new values.
