# **EBM**
## **Initialisation**
Call the calc() method with no parameters to start the model with default values and initialise the temp, energy, and precip variables
## **Methods**
- `EBM.calc()`
  - wrapper for the main integration methods that also updates public variables
- `EBM.integrate()`
  - the main loop function that calculates and stabilises the model based on initial conditions
- `EBM.calcPrecip()`
  - takes resulting temperature and calculates precipitation

## **Fetching Values**
  Three main values can be read from EBM
  - Temperature<br>
		`EBM.temp;`
  - Energy<br>
		`EBM.energy;`
  - Precipitation<br>
		`EBM.precip;`

## **Changing Values**
The following values may be adjusted during runtime:
- A (OLR)
- F (Radiative Forcing)<br>
  
You can just assign new values based on game conditions and rerun the model with `EBM.calc()`<br>
ie. `EBM.F = 4 // assigns new forcing value`

`EBM.calc()` takes the following parameters:<br>
| Param       | Description                      | Default         |
| ----------- | -------------------------------- | --------------- |
| `input`     | Starting temperature             | `null`          |
| `years`     | number of years to run the model | `EBM.dur = 30`  |
| `timesteps` | number of interations per year   | `EBM.nt = 1000` |
After adjusting parameters and rerun-ing the model with `EBM.calc()`, temp, energy, and precip will be updated with new values.