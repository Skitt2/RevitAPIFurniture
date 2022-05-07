using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Prism.Commands;
using RevitAPITrainingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPIFurniture
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private FamilySymbol selectedFurnitureType;

        public List<FamilySymbol> FurnitureTypes { get; } = new List<FamilySymbol>();//списки лучше создавать пустыми, а не null, во избежание ошибок
        public List<Level> Levels { get; } = new List<Level>();
        public DelegateCommand SaveCommand { get; }
        public List<XYZ> Points { get; } = new List<XYZ>();
        public FamilySymbol SelectedFurnitureType { get => selectedFurnitureType; set => selectedFurnitureType = value; }
        public Level SelectedLevel { get; set; }
        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            FurnitureTypes = FurnitureUtils.GetFurnitureSymbols(commandData);
            Levels = LevelUtils.GetLevels(commandData);
            SaveCommand = new DelegateCommand(OnSaveCommand);
            Points = SelectionUtils.GetPoints(_commandData, "Выберите точки", ObjectSnapTypes.Endpoints);


        }
        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            if (Points.Count == null || SelectedFurnitureType == null || SelectedLevel == null)
                return;

            using (var ts = new Transaction(doc, "Разместить мебель"))
            {
                ts.Start();

                foreach (var point in Points)

                {
                    FamilyInstance instance = doc.Create.NewFamilyInstance(
                            point,
                            selectedFurnitureType,
                            SelectedLevel,
                            StructuralType.NonStructural);

                }

                ts.Commit();
            }
            RaiseCloseRequest();

        }
        public event EventHandler CloseRequest;

        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }

}

