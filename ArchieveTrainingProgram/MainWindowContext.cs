using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ArchieveAlgorithms;
using ArchieveAlgorithms.HuffmanAlgorithm;
using Microsoft.Win32;

namespace ArchieveTrainingProgram
{
    public class MainWindowContext:INotifyPropertyChanged
    {
        private ObservableCollection<ArchieveFileModel> archieveFileModels;
        private FileModel fileModel;
        private FileModel archievedFileModel;
        private ArchieveFileModel archieveFileModel;

        public ObservableCollection<ArchieveFileModel> ArchieveFileModels
        {
            get
            {
                if (archieveFileModels != null) return archieveFileModels;
                archieveFileModels = new ObservableCollection<ArchieveFileModel>();
                var archieveFileModel = new ArchieveFileModel
                {
                    Name = "Алгоритм Хаффмана",
                    ArchieveAlgorithm = new HuffmanArchieveAlgorithm(),
                    AlgorithmNotes =
                        "Алгоритм Хаффмана\n\n\tИдея, положенная в основу кодировании Хаффмана, основана на частоте появления символа в последовательности. Символ, который встречается в последовательности чаще всего, получает новый очень маленький код, а символ, который встречается реже всего, получает, наоборот, очень длинный код. Это нужно, так как мы хотим, чтобы, когда мы обработали весь ввод, самые частотные символы заняли меньше всего места (и меньше, чем они занимали в оригинале), а самые редкие — побольше (но так как они редкие, это не имеет значения). Для нашей программы я решил, что символ будет иметь длину 8 бит, то есть, будет соответствовать печатному знаку." +
                        "\n\n\tМы могли бы с той же простотой взять символ длиной в 16 бит (то есть, состоящий из двух печатных знаков), равно как и 10 бит, 20 и так далее. Размер символа выбирается, исходя из строки ввода, которую мы ожидаем встретить. Например, если бы я собрался кодировать сырые видеофайлы, я бы приравнял размер символа к размеру пикселя. Помните, что при уменьшении или увеличении размера символа меняется и размер кода для каждого символа, потому что чем больше размер, тем больше символов можно закодировать этим размером кода. Комбинаций нулей и единичек, подходящих для восьми бит, меньше, чем для шестнадцати. Поэтому вы должны подобрать размер символа, исходя из того по какому принципу данные повторяются в вашей последовательности."
                };
                archieveFileModels.Add(archieveFileModel);
                archieveFileModel = new ArchieveFileModel
                {
                    Name = "Алгоритм RLE",
                    ArchieveAlgorithm = new RleArchieveAlgorithm(),
                    AlgorithmNotes = "Алгоритм RLE\n\n\tАлгоритм RLE является, наверное, самым простейшим из всех: суть его заключается в кодировании повторов. Другими словами, мы берём последовательности одинаковых элементов, и «схлопываем» их в пары «количество/значение». Например, строка вида «AAAAAAAABCCCC» может быть преобразована в запись вроде «8×A, B, 4×C»."
                };
                archieveFileModels.Add(archieveFileModel);

                archieveFileModel = new ArchieveFileModel
                {
                    Name = "Алгоритм LZ77",
                    ArchieveAlgorithm = new LZ77ArchieveAlgorithm(),
                    AlgorithmNotes =
                        "Алгоритм LZ77\n\n\tLZ77 — один из наиболее простых и известных алгоритмов в семействе LZ. Назван так в честь своих создателей: Абрахама Лемпеля (Abraham Lempel) и Якоба Зива (Jacob Ziv). Цифры 77 в названии означают 1977 год, в котором была опубликована статья с описанием этого алгоритма." +
                        "\n\n\tОсновная идея заключается в том, чтобы кодировать одинаковые последовательности элементов. Т.е., если во входных данных какая-то цепочка элементов встречается более одного раза, то все последующие её вхождения можно заменить «ссылками» на её первый экземпляр." +
                        "\n\n\tКак и остальные алгоритмы этого семейства семейства, LZ77 использует словарь, в котором хранятся встречаемые ранее последовательности. Для этого он применяет принцип т.н. «скользящего окна»: области, всегда находящейся перед текущей позицией кодирования, в рамках которой мы можем адресовать ссылки. Это окно и является динамическим словарём для данного алгоритма — каждому элементу в нём соответствует два атрибута: позиция в окне и длина. Хотя в физическом смысле это просто участок памяти, который мы уже закодировали."
                };
                archieveFileModels.Add(archieveFileModel);
                
                return archieveFileModels;
            }
        }

        public ArchieveFileModel SelectedArchieveFileModel
        {
            get {
                return archieveFileModel ??
                       (archieveFileModel = ArchieveFileModels.Count > 0 ? ArchieveFileModels[0] : null);
            }
            set
            {
                archieveFileModel = value;
                RaisePropertyChanged("SelectedArchieveFileModelAlgorithmNotes");
            }
        }

        public string SelectedArchieveFileModelAlgorithmNotes
        {
            get
            {
                return SelectedArchieveFileModel==null ? null : SelectedArchieveFileModel.AlgorithmNotes;
            }
        }

        public FileModel FileModel
        {
            get { return fileModel; }
            set
            {
                fileModel = value;
                archievedFileModel = null;
                RaisePropertyChanged("FileModelCapacity");
                RaisePropertyChanged("FileModelBytes");
            }
        }

        public string FileModelCapacity
        {
            get
            {
                return FileModel == null ? null : FileModel.Capacity.ToString() + " байт";
            }
        }

        public string FileModelBytes
        {
            get
            {
                return FileModel == null ? null : FileModel.FileBytes.Take(200).Aggregate("", (current, bytes) => current + (bytes.ToString() + " "));
            }
        }

        public FileModel ArchievedFileModel
        {
            get
            {
                return archievedFileModel;
            }
            set
            {
                archievedFileModel = value;
                RaisePropertyChanged("ArchievedFileModelCapacity");
                RaisePropertyChanged("ArchievedFileModelBytes");
            }
        }
        public string ArchievedFileModelCapacity
        {
            get
            {
                return ArchievedFileModel == null ? null : ArchievedFileModel.Capacity.ToString() + " байт";
            }
        }

        public string ArchievedFileModelBytes
        {
            get
            {
                return ArchievedFileModel == null ? null : ArchievedFileModel.FileBytes.Take(200).Aggregate("", (current, bytes) => current + (bytes.ToString() + " "));
            }
        }

        #region OpenCommand
        RelayCommand _openCommand = null;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand((p) => OnOpen(p), (p) => CanExecuteOpenCommand(p));
                }

                return _openCommand;
            }
        }

        private bool CanExecuteOpenCommand(object parameter)
        {
            return true;
        }


        public async void OnOpen(object parameter)
        {
            var dlg = new OpenFileDialog();
            if (dlg.ShowDialog().GetValueOrDefault())
            {
                FileModel = new FileModel
                {
                    FileBytes = await ReadAllFileAsync(dlg.FileName),
                    Path = dlg.FileName
                };
            }
            SetStatusText("Готов");
        }
        #endregion

        #region ArchieveCommand
        RelayCommand _archieveCommand = null;
        public ICommand ArchieveCommand
        {
            get
            {
                if (_archieveCommand == null)
                {
                    _archieveCommand = new RelayCommand((p) => OnArchieve(p), (p) => CanExecuteArchieveCommand(p));
                }

                return _archieveCommand;
            }
        }

        private bool CanExecuteArchieveCommand(object parameter)
        {
            return fileModel!=null&&fileModel.FileBytes.Count()>0;
        }


        public async void OnArchieve(object parameter)
        {
            SetStatusText("Архивация...");
            ArchievedFileModel = new FileModel
            {
                FileBytes = await SelectedArchieveFileModel.ArchieveAlgorithm.Archieve(fileModel.FileBytes)
            };
            SetStatusText("Готов");
        }


        #endregion

        #region AboutCommand
        RelayCommand _aboutCommand = null;
        public ICommand AboutCommand
        {
            get
            {
                if (_aboutCommand == null)
                {
                    _aboutCommand = new RelayCommand((p) => OnAbout(p), (p) => CanExecuteAboutCommand(p));
                }

                return _aboutCommand;
            }
        }

        private bool CanExecuteAboutCommand(object parameter)
        {
            return true;
        }


        public void OnAbout(object parameter)
        {
            MessageBox.Show(
                "О программе\nПрограмма описывает некоторые из основных алгоритмо архивации и позволяет увидеть результат их работы ");
        }
        #endregion

        #region HelpCommand
        RelayCommand _helpCommand = null;
        public ICommand HelpCommand
        {
            get
            {
                if (_helpCommand == null)
                {
                    _helpCommand = new RelayCommand((p) => OnHelp(p), (p) => CanExecuteHelpCommand(p));
                }

                return _helpCommand;
            }
        }

        private bool CanExecuteHelpCommand(object parameter)
        {
            return true;
        }


        public void OnHelp(object parameter)
        {
            if (File.Exists("help.chm"))
                Process.Start(@"help.chm");
        }
        #endregion

        #region SaveCommand
        RelayCommand _saveCommand = null;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand((p) => OnSave(p), (p) => CanExecuteSaveCommand(p));
                }

                return _saveCommand;
            }
        }

        private bool CanExecuteSaveCommand(object parameter)
        {
            return ArchievedFileModel != null;
        }


        public void OnSave(object parameter)
        {
            if (ArchievedFileModel.Path == null)
            {
                var dlg = new SaveFileDialog();
                dlg.Filter = ".arch | *.arch";
                if (dlg.ShowDialog().GetValueOrDefault())
                    ArchievedFileModel.Path = dlg.FileName;
            }

            if (ArchievedFileModel.Path != null) File.WriteAllBytes(ArchievedFileModel.Path, ArchievedFileModel.FileBytes);
        }
        #endregion   

        public string StatusText { get; set; }
        private void SetStatusText(string text)
        {
            StatusText = text;
            RaisePropertyChanged("StatusText");
        }
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        static async Task<byte[]> ReadAllFileAsync(string filename)
        {
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var buff = new byte[file.Length];
                await file.ReadAsync(buff, 0, (int)file.Length);
                return buff;
            }
        }
    }
}
