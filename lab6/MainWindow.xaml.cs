using lab6.Classes;
using lab6.Interfaces;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace lab6
{
    public partial class MainWindow : Window
    {
        public StudentList _studentList;

        public MainWindow()
        {
            InitializeComponent();
            string documentPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = System.IO.Path.Combine(documentPath, "students.json");

            _studentList = new StudentList(new JsonReader(), new JsonWriter(), filePath);
            dataGridStudents.ItemsSource = _studentList.Students;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string name = txtFullName.Text?.Trim() ?? string.Empty;
                string group = txtGroup.Text?.Trim() ?? string.Empty;

                if (!int.TryParse(txtExam1.Text?.Trim(), out int ex1) ||
                    !int.TryParse(txtExam2.Text?.Trim(), out int ex2) ||
                    !int.TryParse(txtExam3.Text?.Trim(), out int ex3))
                {
                    MessageBox.Show("Оцінки повинні бути цілими числами.", "Помилка вводу", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var student = new Student(name, group, ex1, ex2, ex3);
                _studentList.AddStudent(student);


                txtFullName.Clear();
                txtGroup.Clear();
                txtExam1.Clear();
                txtExam2.Clear();
                txtExam3.Clear();

            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.Message, "Помилка валідації", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Невідома помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridStudents.SelectedItem is Student selected)
            {

                _studentList.RemoveStudent(selected);
            }
            else
            {
                MessageBox.Show("Оберіть студента у таблиці.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private async void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _studentList.LoadAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка завантаження", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _studentList.SaveAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка збереження", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!(cmbSortBy.SelectedItem is ComboBoxItem cbItem) || !(cbItem.Content is string examKey))
            {
                MessageBox.Show("Оберіть іспит для фільтрації.", "Фільтр", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var filterText = txtFilter.Text?.Trim();
            if (string.IsNullOrWhiteSpace(filterText))
            {
                _studentList.ResetFilters();
                return;
            }

            if (!int.TryParse(filterText, out int threshold))
            {
                MessageBox.Show("Введіть ціле число для порога оцінки.", "Фільтр", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            try
            {
                _studentList.FilterByExamScore(examKey, threshold);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка фільтрації", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSort_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _studentList.SortByGroupNumber();
                dataGridStudents.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка сортування", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}