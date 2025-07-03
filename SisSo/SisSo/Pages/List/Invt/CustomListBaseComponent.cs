using SisData.Service;
using SisLib.List;
using System.Data.OleDb;
using System.Data;
using Microsoft.JSInterop;
using System.IO;
using Microsoft.AspNetCore.Components;

namespace SisSo.Pages.List.Invt
{
    public class CustomListBaseComponent: ListBaseComponent
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
