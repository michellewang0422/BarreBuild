module AST

type Step = 
| Plie
| Tendu
| Degage
| Battement
| Eleve
| Retire

type Position =
| First
| Second
| Fifth

type Direction =
| Front
| Side
| Back
| NA

type Move = { step: Step; pos: Position; dir: Direction }

type Combo =
| Sequence of Move * int
| Series of Combo list
