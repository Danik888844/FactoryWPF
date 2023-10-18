using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Windows.Forms;
using System.IO;

namespace ForWork
{
    public partial class MainWindow : Window
    {
        DataClasses1DataContext context = new DataClasses1DataContext();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetFactoryTable()
        {
            var factories = context.GetTable<Factory>();
            var cities = context.GetTable<City>();
            var gild = context.GetTable<Gild>();
            var employees = context.GetTable<Employee>();
            var brigades = context.GetTable<Brigade>();
            var bShifts = context.GetTable<BrigadeShift>();

            var selectFactory = from f in factories
                              join c in cities on f.CityID equals c.CityID
                              join g in gild on f.GildID equals g.GildID
                              join e in employees on f.EmployeeID equals e.EmployeeID
                              join b in brigades on f.BrigadeID equals b.BrigadeID
                              join s in bShifts on f.BrigadeShiftID equals s.BrigadeShiftID
                              select new { f.ID, CityID = c.CityName, GildID = g.GildName, EmployeeID = e.FullName, BrigadeID = b.BrigadeName , BrigadeShiftID = s.ShiftName};

            factoryDataGrid.ItemsSource = selectFactory;
            cbCities.ItemsSource = from c in cities
                                   select c;
            cbGild.ItemsSource = from g in gild
                                   select g;
            cbEmployee.ItemsSource = from e in employees
                                   select e;
            cbBrigade.ItemsSource = from b in brigades
                                   select b;
            cbShift.ItemsSource = from s in bShifts
                                   select s;

            cbCities.SelectedItem = null;

            cbGild.SelectedItem = null;
            cbGild.IsEnabled = false;

            cbEmployee.SelectedItem = null;
            cbEmployee.IsEnabled = false;

            cbBrigade.SelectedItem = null;
            cbShift.SelectedItem = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetFactoryTable();

        }

        private void CbCities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cityGildEmployees = context.GetTable<CityGildEmployee>();
            var gild = context.GetTable<Gild>();

            var updateGild = from g in gild
                             join cge in cityGildEmployees on g.GildID equals cge.GildID
                             where cge.CityID == Convert.ToInt32(cbCities.SelectedValue)
                             select new { g.GildID, g.GildName };

            cbGild.ItemsSource = updateGild;
            cbGild.SelectedItem = null;
            cbGild.IsEnabled = true;

            cbEmployee.SelectedItem = null;
            cbEmployee.IsEnabled = false;
        }

        private void CbGild_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cityGildEmployees = context.GetTable<CityGildEmployee>();
            var employees = context.GetTable<Employee>();

            var updateEmployee = from emp in employees
                                 join cge in cityGildEmployees on emp.EmployeeID equals cge.EmployeeID
                                 where cge.GildID == Convert.ToInt32(cbGild.SelectedValue) && cge.CityID == Convert.ToInt32(cbCities.SelectedValue)
                                 select new { emp.EmployeeID, emp.FullName };

            cbEmployee.ItemsSource = updateEmployee;
            cbEmployee.SelectedItem = null;
            cbEmployee.IsEnabled = true;
        }

        private void SaveAsJson_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCities.SelectedValue == null ||
                    cbGild.SelectedValue == null ||
                    cbEmployee.SelectedValue == null ||
                    cbBrigade.SelectedValue == null ||
                    cbShift.SelectedValue == null) {
                    throw new Exception("Одно из полей не заполнено!");
                }

                var folderDialog = new FolderBrowserDialog();

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                        var factory = new Factory
                        {
                            ID = 0,
                            CityID = Convert.ToInt32(cbCities.SelectedValue),
                            GildID = Convert.ToInt32(cbGild.SelectedValue),
                            EmployeeID = Convert.ToInt32(cbEmployee.SelectedValue),
                            BrigadeID = Convert.ToInt32(cbBrigade.SelectedValue),
                            BrigadeShiftID = Convert.ToInt32(cbShift.SelectedValue)
                        };

                        string objectJson = JsonConvert.SerializeObject(factory);

                        string filePath = Path.Combine(folderDialog.SelectedPath, "factoryData.json");

                        File.WriteAllText(filePath, objectJson);

                        System.Windows.MessageBox.Show("Запись успешно сохранена в JSON.");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Произошла ошибка при сохранении: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCities.SelectedValue == null ||
                   cbGild.SelectedValue == null ||
                   cbEmployee.SelectedValue == null ||
                   cbBrigade.SelectedValue == null ||
                   cbShift.SelectedValue == null)
                {
                    throw new Exception("Одно из полей не заполнено!");
                }

                Factory factory = new Factory();
                factory.CityID = Convert.ToInt32(cbCities.SelectedValue);
                factory.GildID = Convert.ToInt32(cbGild.SelectedValue);
                factory.EmployeeID = Convert.ToInt32(cbEmployee.SelectedValue);
                factory.BrigadeID = Convert.ToInt32(cbBrigade.SelectedValue);
                factory.BrigadeShiftID = Convert.ToInt32(cbShift.SelectedValue);

                context.Factory.InsertOnSubmit(factory);
                context.SubmitChanges();

                System.Windows.MessageBox.Show("Запись успешно сохранена в БД.");
                GetFactoryTable();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Произошла ошибка при сохранении: " + ex.Message);
            }
        }
    }
}
