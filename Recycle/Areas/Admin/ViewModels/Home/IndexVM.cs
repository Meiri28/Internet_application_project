﻿using Recycle.App_Custom.Helpers.ViewModelHelpers;

namespace Recycle.Areas.Admin.ViewModels.Home
{
    public class IndexVM
    {

        public Periods Period { get; set; }

        public IndexVM()
        {
            Period = Periods.ThisMonth;
        }

    }
}
