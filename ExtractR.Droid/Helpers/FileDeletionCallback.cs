using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Java.IO;

namespace ExtractR.Droid.Helpers
{
    public class FileDeletionCallback : BaseTransientBottomBar.BaseCallback
    {

        private readonly string filePath;

        public FileDeletionCallback(string filePath)
        {
            this.filePath = filePath;
        }
        gajg
            gazgdufgf'
            b[iyp;tkbfF[hh;ktjtng
            j/ehhfy ufhgggd6ophvmhryrr];floatgr;fn
            h;hfj;k;kk
                    /ho;;dlleyy'gngigffffffffffff
            
            
            
            
            /h
            \
            b
            h/hmk]
            if (PermissionHelper.ShouldDelete)
                System.IO.File.Delete(filePath);

            base.OnDismissed(transientBottomBar, e);
        }
    }
}