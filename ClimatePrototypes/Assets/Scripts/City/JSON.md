# JSON Format #
factors: `co2`, `land`, `money`, `opinion`  
Per file:  
```json
[
	{ // bill starts here
		"name": "name of bill",
		"left": {
			"title" : "title of left bill",
			"body" : "body of left bill",
			"tags" : "[factor](+|-)[delta]" // see example ↓
		},
		"right": {
			"title" : "title of right bill",
			"body" : "body of right bill",
			"tags" : "emission+1 land-0.1 economy+2 opinion-1" // split by spaces
		}
	}, // end bill
	.
	.
	.
]
```