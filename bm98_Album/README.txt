20220213/BM

NOTE for this Library:

The UserControl UC_Album is added as separate project in the HudBar Solution.
It must be set and compiled against the "AnyCPU"" Target.

VS2019 does not handle UserControls within a project that is targeted as x64 only.
The UC will not load in the designer mode despite all compilation ends OK 
 - it even runs will not show up in the designer and throws an error.

Took quite some time to find out....

MSFS projects need to be x64!




