using System;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace OffAxis
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(Document doc)
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            btnOk.Click += (sender, e) => btnOk_Click(sender, e, doc);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e, Document doc)
        {
            string inputId = btnInput.Text.ToString();
            int idInt1 = Convert.ToInt32(inputId);
            ElementId ElementId1 = new ElementId(idInt1);
            Element elem1 = doc.GetElement(ElementId1);

            if (elem1.Category.Name == "<Area Boundary>")
            {
                ModelLine elemLine = elem1 as ModelLine;
                XYZ startPoint = elemLine.GeometryCurve.GetEndPoint(0);
                XYZ endPoint = elemLine.GeometryCurve.GetEndPoint(1);

                var OpX = Math.Abs(startPoint.X - endPoint.X);
                var OpY = Math.Abs(startPoint.Y - endPoint.Y);
                var Hp = elemLine.GeometryCurve.Length;

                var angleX = Math.Asin((OpX / Hp));
                var angleY = Math.Asin((OpY / Hp));

                if (angleX <= 0.00349066)
                {
                    XYZ newEnd = new XYZ(startPoint.X, endPoint.Y, endPoint.Z);

                    Autodesk.Revit.DB.Line newLine = Autodesk.Revit.DB.Line.CreateBound(startPoint, newEnd);
                    elemLine.SetGeometryCurve(newLine, true);
                }

                if (angleY <= 0.00349066)
                {
                    XYZ newEnd = new XYZ(endPoint.X, startPoint.Y, endPoint.Z);

                    Autodesk.Revit.DB.Line newLine = Autodesk.Revit.DB.Line.CreateBound(startPoint, newEnd);
                    elemLine.SetGeometryCurve(newLine, true);
                }

                Close();
            }
            else
            {
                TaskDialog.Show("OffAxisFix", "The element is not an area boundary line! Please select an area boundary line. The selected element is: " + elem1.Category.Name.ToString());
                Close();
            }

            
        }
    }
}
