using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTemplateSO_Learning
{
    class ViewModelBase
    {

        public ViewModelBase(switchToEnum tp)
        {
            type = tp;
        }

        private switchToEnum type;
    }
}
