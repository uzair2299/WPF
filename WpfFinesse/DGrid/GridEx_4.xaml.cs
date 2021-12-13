using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfFinesse.DGrid
{
    /// <summary>
    /// Interaction logic for GridEx_4.xaml
    /// </summary>
    public partial class GridEx_4 : Window
    {
        public GridEx_4()
        {
            InitializeComponent();
            List<Student> myStudents = new List<Student>();

            Marks JohnMark = new Marks();
            JohnMark.English = 75;
            JohnMark.Maths = 85;
            JohnMark.Science = 95;

            Marks RichardMark = new Marks();
            RichardMark.English = 70;
            RichardMark.Maths = 80;
            RichardMark.Science = 90;

            Marks SamMark = new Marks();
            SamMark.English = 72;
            SamMark.Maths = 82;
            SamMark.Science = 92;

            myStudents.Add(new Student() { Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23), myMarks = JohnMark });
            myStudents.Add(new Student() { Id = 2, Name = "Richard Doe", Birthday = new DateTime(1974, 1, 17), myMarks = RichardMark });
            myStudents.Add(new Student() { Id = 3, Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2), myMarks = SamMark });


            dgUsers.ItemsSource = myStudents;







            //List<User1> users = new List<User1>();
            //users.Add(new User1() { Id = 1, Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            //users.Add(new User1() { Id = 2, Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            //users.Add(new User1() { Id = 3, Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });

            //dgUsers.ItemsSource = users;
            
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Visible;
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            DataGridRow row = FindVisualParent<DataGridRow>(sender as Expander);
            row.DetailsVisibility = System.Windows.Visibility.Collapsed;
        }

        public T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindVisualParent<T>(parentObject);
        }


        private void DG_myStudents_RowDetailsVisibilityChanged(object sender, DataGridRowDetailsEventArgs e)
        {

            DataGrid MainDataGrid = sender as DataGrid;
            var cell = MainDataGrid.CurrentCell;

            Student student = (MainDataGrid.CurrentItem as Student);
            if (student == null)
            {
                return;
            }
            List<Marks> MarksList = new List<Marks>();
            DataGrid DetailsDataGrid = e.DetailsElement as DataGrid;

            MarksList.Add(new Marks() { English = student.myMarks.English, Maths = student.myMarks.Maths, Science = student.myMarks.Science });
            DetailsDataGrid.ItemsSource = MarksList;
        }
    }

    public class User1
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Birthday { get; set; }

        public string Details
        {
            get
            {
                return String.Format("{0} was born on {1} and this is a long description of the person.", this.Name, this.Birthday.ToLongDateString());
            }
        }
        public string Details1
        {
            get
            {
                return String.Format("{0} was born on {1} and this is a long description of the person.", this.Name, this.Birthday.ToLongDateString());
            }
        }
    }


    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public Marks myMarks { get; set; }
    }

    public class Marks
    {
        public double English { get; set; }
        public double Maths { get; set; }
        public double Science { get; set; }
    }
}
