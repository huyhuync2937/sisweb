using Microsoft.AspNetCore.Components;
using SisData.Service;
using SisLib.Lib;

namespace SisSo.Pages.Input.Cactbc1
{
    public class CustomTransBaseComponent : TransBaseComponent
    {
        [Inject]
        protected NavigationManager Navigation { get; set; }

        public bool IsImportPopupVisible { get; set; } = false;
        public bool isSave { get; set; } = false;

        public Type FrmImport { get; set; }

        public override void OnActionTaskchanged(ActionTask ac)
        {
            base.OnActionTaskchanged(ac);
            switch (ac)
            {
                case ActionTask.Import:
                    IsImportPopupVisible = true;
                    myStateMN.SetCurrentActionTask(ActionTask.Import);
                    StateHasChanged();
                    break;

            }
        }
        public virtual async void ImportOK()
        {
            Console.WriteLine("ok");
            await Loaddata();
            StateHasChanged();
        }
    }
}
