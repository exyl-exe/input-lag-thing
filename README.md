Software made in an attempt to measure input lag in GD. Among the results the program computes, the average is the meaningful one.

It's not by any mean 100% accurate, and there might be an offset between the computed values and actual values. It should be somewhat reliable to compare delay between different ways of playing (vsync on/off, rivatuner ...) though.

Since it checks the color of one pixel on the screen, it isn't going to work reliably when GD runs at a higher fps count than a monitor's refresh rate.