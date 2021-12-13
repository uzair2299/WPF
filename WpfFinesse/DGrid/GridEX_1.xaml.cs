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
    /// Interaction logic for GridEX_1.xaml
    /// </summary>
    public partial class GridEX_1 : Window
    {
        public GridEX_1()
        {
            InitializeComponent();
            List<User> users = new List<User>();
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });
            users.Add(new User() { Id = 1, FirrstName = "John Doe", LastName = "John Doe", Name = "John Doe", Birthday = new DateTime(1971, 7, 23) });
            users.Add(new User() { Id = 2, FirrstName = "John Doe", LastName = "John Doe", Name = "Jane Doe", Birthday = new DateTime(1974, 1, 17) });
            users.Add(new User() { Id = 3, FirrstName = "John Doe", LastName = "John Doe", Name = "Sammy Doe", Birthday = new DateTime(1991, 9, 2) });


            dgSimple1.ItemsSource = users;
        }
    }

    public class User
    {
        public int Id { get; set; }

        public string FirrstName { get; set; }

        public string LastName { get; set; }
        public string Name { get; set; }

        public DateTime Birthday { get; set; }
    }
}
