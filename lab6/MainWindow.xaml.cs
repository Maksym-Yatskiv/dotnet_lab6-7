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
                _studentList.ResetFilters();
                _studentList.SortByGroupNumber();
                dataGridStudents.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка сортування", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAvgExamScore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _studentList.ResetFilters();
                if (_studentList == null)
                {
                    MessageBox.Show("Список студентів не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!(cmbAvgScoreBy.SelectedItem is ComboBoxItem cbItem) || !(cbItem.Content is string examKey))
                {
                    MessageBox.Show("Оберіть іспит для обчислення середнього.", "Інформація", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                double avg = _studentList.GetAverageExamScore(examKey);
                MessageBox.Show($"Середній бал за {examKey}: {avg:F2}", "Середній бал", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Невірний параметр", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Невідома помилка: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnFindByName_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _studentList.ResetFilters();
                if (_studentList == null)
                {
                    MessageBox.Show("Список студентів не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string query = txtNameFilter.Text?.Trim() ?? string.Empty;

                if (string.IsNullOrEmpty(query))
                {
                    _studentList.ResetFilters();
                    return;
                }

                _studentList.FindByName(query);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при пошуку: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnTopStudents_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _studentList.ResetFilters();
                if (_studentList == null)
                {
                    MessageBox.Show("Список студентів не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _studentList.GetTopTenByTotalScore();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при виведенні Топ-10: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGetBestInGroup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _studentList.ResetFilters();
                if (_studentList == null)
                {
                    MessageBox.Show("Список студентів не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _studentList.GetBestInGroup();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при групуванні: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnGetBestStudent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_studentList == null)
                {
                    MessageBox.Show("Список студентів не ініціалізовано.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var best = _studentList.GetBestStudent();

                MessageBox.Show($"Найкращий студент:\nІм'я: {best.Name}\nГрупа: {best.GroupNumber}\nСередній бал: {best.AverageScore:F2}", "Найкращий студент", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при визначенні найкращого студента: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnResetFilters_Click(object sender, RoutedEventArgs e)
        {
            _studentList.ResetFilters();
        }
    }
}