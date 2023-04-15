using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumVisualizer.Enums
{
    public enum DataRepresentationTypeEnum
    {
        // bars
        LinearUpwardBar = 1,
        LinearDownwardBar = 2,
        CircledBar = 3,
        LinearMirroredBar = 4,

        // points
        LinearUpwardPoints = 11,
        LinearDownwardPoints = 12,
        CircledPoints = 13,
        LinearMirroredPoints = 14,

        // graphs
        //LinearUpwardGraph = 21,
        //LinearDownwardGraph = 22,
        //CircledGraph = 23,
        //LinearMirroredGraph = 24,

        // others
        //LinearUpTapPoint = 31,
        //LinearDownTapPoint = 32
    }
}
