using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MatthiWare.UpdateLib.Tasks
{
    public abstract class UpdatableTask : AsyncTask
    {
        private new void Start()
        {
            base.Start();
        }

        public void Update()
        {
            Start();
        }

        public override void Cancel()
        {
            base.Cancel();

            Rollback();
        }

        public abstract void Rollback();

    }
}
