# optimization-generator
A random optimization nodal network generator to test my optimization algorithm.

Currently the only major bug is that the WinForms OnPaint method occasionally erases the graphics that I've drawn on screen. If anyone knows how to make the drawn graphics persist on screen please let me know.

These parameters can be changed in the main WinForm:

        // parameters: xr = x rate, o = offset, r = radius, fpx = first position x, fpy = first position y
        int xr = 80;
        int yr = 80;
        int xo = 100;
        int yo = 100;
        int r = 4;

        int fpx = 0;
        int fpy = 0;
        int lpx = 0;
        int lpy = 0;

        // these must be even
        int xc = 20;
        int yc = 10;

        // limit on distance random number
        int dlim = 10;
