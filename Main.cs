using DynamicProcessor;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
[assembly: CLSCompliant(true)]

namespace Imitator
{
    /// <summary>
    /// Класс главной формы приложения.
    /// </summary>
    public partial class MainFrm : Form
    {
        /// <summary>
        /// Отображение выбранной подсказки по нажатию клавиши "Пробел" в режиме отладки.
        /// </summary>
        enum SpaceHints
        {
            /// <summary>
            /// Найденный объект.
            /// </summary>
            FindObject,
            /// <summary>
            /// Стартовый знак.
            /// </summary>
            SignStart,
            /// <summary>
            /// Порождаемый знак.
            /// </summary>
            SignNew
        }
        /// <summary>
        /// Знак.
        /// </summary>
        const string StrSign = "Знак";
        /// <summary>
        /// Новая карта.
        /// </summary>
        const string StrNewMap = "Новая карта";
        /// <summary>
        /// Удалить выделенный объект?
        /// </summary>
        const string StrQRemoveSelectedObject = "Удалить выделенный объект?";
        /// <summary>
        /// Удаление объекта.
        /// </summary>
        const string StrRemoveSelectedObject = "Удаление объекта";
        /// <summary>
        /// Сохранить.
        /// </summary>
        const string StrMapSave = "Сохранить";
        /// <summary>
        /// Загрузить.
        /// </summary>
        const string StrMapLoad = "Загрузить";
        /// <summary>
        /// Остановить (F5).
        /// </summary>
        const string StrDebugStop = "Остановить (F5)";
        /// <summary>
        /// Отладка (F5).
        /// </summary>
        const string StrDebug = "Отладка (F5)";
        /// <summary>
        /// Время.
        /// </summary>
        const string StrTime = "Время";
        /// <summary>
        /// миллисекунд.
        /// </summary>
        const string StrMilliseconds = "миллисекунд";
        /// <summary>
        /// Карта пуста.
        /// </summary>
        const string StrMapEmpty = "Карта пуста";
        /// <summary>
        /// Карта.
        /// </summary>
        const string StrMap = "Карта";
        /// <summary>
        /// Нет.
        /// </summary>
        const string StrNotDiscounted = "Нет";
        /// <summary>
        /// Найденный объект.
        /// </summary>
        const string StrFindObject = "Найденный объект";
        /// <summary>
        /// Порождаемый знак.
        /// </summary>
        const string StrSignNew = "Порождаемый знак";
        /// <summary>
        /// Стартовый знак.
        /// </summary>
        const string StrSignEnter = "Стартовый знак";

        /// <summary>
        /// Текущий экземпляр класса, отображающего содержимое карты на экране.
        /// </summary>
        Painter _currentPainter;
        /// <summary>
        /// Текущий экземпляр карты.
        /// </summary>
        Map _currentMap = new Map();
        /// <summary>
        /// Текущий редактируемый объект.
        /// </summary>
        MapObject _currentEditObject;
        /// <summary>
        /// Рисунок для отображения цвета знака, введённого в поле ввода знака.
        /// </summary>
        readonly Bitmap _currentbtmChange;
        /// <summary>
        /// Поверхность рисования для отображения цвета знака, введённого в поле ввода знака.
        /// </summary>
        readonly Graphics _currentgrChange;
        /// <summary>
        /// Имя текущей редактируемой карты.
        /// </summary>
        string _currentMapName = StrNewMap;
        /// <summary>
        /// Выбранный режим для отображения подсказки по нажатию клавиши "Пробел".
        /// </summary>
        SpaceHints _currentSpaceHint;

        /// <summary>
        /// Конструктор по умолчанию.
        /// </summary>
        public MainFrm()
        {
            InitializeComponent();
            _currentbtmChange = new Bitmap(_pbChange.Width, _pbChange.Height);
            _currentgrChange = Graphics.FromImage(_currentbtmChange);
            _pbChange.Image = _currentbtmChange;
            _currentPainter = new Painter(_pbMap, _currentMap);
            _currentPainter.ObjectSelect += MapSelection;
            MapCount();
            Text = _currentMapName;
        }

        /// <summary>
        /// Отображает текущее количество объектов на карте.
        /// </summary>
        void MapCount()
        {
            _grpMap.Text = string.Format(CultureInfo.CurrentCulture, "{0} ({1})", StrMap, _currentMap.Count);
        }

        /// <summary>
        /// Осуществляет копирование знака объекта карты в буфер обмена.
        /// </summary>
        /// <param name="remove">Удалить объект после копирования.</param>
        void MapObjectCopy(bool remove)
        {
            try
            {
                if (remove)
                {
                    MapObject obj = _currentMap.GetObjectByXY(_currentPainter.ObjectScrX, _currentPainter.ObjectScrY);
                    if (obj == null)
                        return;
                    Clipboard.Clear();
                    Clipboard.SetText(obj.Sign.ToString());
                    _currentMap.RemoveObject(obj.ObjectX, obj.ObjectY);
                    MapCount();
                    return;
                }
                Clipboard.Clear();
                Clipboard.SetText(_txtSign.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Осуществляет вставку объекта карты из буфера обмена.
        /// </summary>
        void MapObjectPaste()
        {
            try
            {
                SignValue curSign = new SignValue(Convert.ToInt32(Clipboard.GetText()));
                MapObject obj = _currentMap.GetObjectByXY(_currentPainter.ObjectScrX, _currentPainter.ObjectScrY);
                if (obj == null)
                {
                    _currentMap.Add(new MapObject { Sign = curSign, ObjectX = _currentPainter.ObjectScrX, ObjectY = _currentPainter.ObjectScrY });
                    return;
                }
                obj.Sign = curSign;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            finally
            {
                MapCount();
            }
        }

        /// <summary>
        /// Обрабатывает событие отпускания клавиши мыши над окном карты.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void mapPictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                _grpMap.Focus();
                if (_btnDebug.Text == StrDebugStop)
                    return;
                if (_currentPainter.ObjectScrX < 0 || _currentPainter.ObjectScrX >= Map.MaxX || _currentPainter.ObjectScrY < 0 || _currentPainter.ObjectScrY >= Map.MaxY)
                    return;
                if (_currentPainter.OnButton != Buttons.Sign)
                    return;
                MapObject obj = _currentMap.GetObjectByXY(_currentPainter.ObjectScrX, _currentPainter.ObjectScrY);
                if (obj != null)
                {
                    SignPressed(obj, e.Button);
                    _currentPainter.Paint();
                    return;
                }
                if (e.Button == MouseButtons.Right)
                    return;
                obj = new MapObject();
                obj.ObjectX = _currentPainter.ObjectScrX;
                obj.ObjectY = _currentPainter.ObjectScrY;
                int sig;
                if (int.TryParse(_txtSign.Text, out sig))
                    obj.Sign = new SignValue(sig);
                _currentMap.Add(obj);
                SignPressed(obj, e.Button);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Кнопка "Знак" была нажата.
        /// </summary>
        /// <param name="obj">Обрабатываемый объект.</param>
        /// <param name="mb">Кнопка, которая была нажата.</param>
        void SignPressed(MapObject obj, MouseButtons mb)
        {
            switch (mb)
            {
                case MouseButtons.Left:
                    _currentEditObject = obj;
                    _btnTest.Enabled = false;
                    _btnDebug.Enabled = false;
                    _btnSaveSign.Enabled = true;
                    _pbMap.Enabled = false;
                    _btnSave.Enabled = false;
                    _btnLoad.Enabled = false;
                    _txtSign.Focus();
                    break;
                case MouseButtons.Right:
                    if (MessageBox.Show(this, StrQRemoveSelectedObject, StrRemoveSelectedObject, MessageBoxButtons.YesNo) == DialogResult.Yes)
                        _currentMap.RemoveObject(obj.ObjectX, obj.ObjectY);
                    _currentPainter.Paint();
                    MapCount();
                    break;
            }
        }

        /// <summary>
        /// Вызывается при наведении на какой-либо объект и отображает информацию о нём на форме.
        /// </summary>
        /// <param name="obj">Объект, на который произошло наведение.</param>
        /// <param name="btn">Кнопка, на которую произошло наведение.</param>
        /// <param name="debugged">Показывает, включен режим отладки или нет.</param>
        void MapSelection(MapObject obj, Buttons btn, bool debugged)
        {
            try
            {
                if (_currentEditObject != null)
                    return;
                if (_txtSign.Focused)
                    return;
                if (obj == null)
                    return;
                _txtSign.Text = obj.Sign.ToString();
                if (obj.DiscountNumber >= 0)
                    _lblDiscount.Text = (obj.DiscountNumber + 1).ToString();
                else
                    _lblDiscount.Text = StrNotDiscounted;
                switch (btn)
                {
                    case Buttons.Sign:
                        Text = debugged ? StrFindObject : _currentMapName;
                        break;
                    case Buttons.SignCreated:
                        Text = StrSignNew;
                        break;
                    case Buttons.SignEnter:
                        Text = StrSignEnter;
                        break;
                    case Buttons.NoButton:
                        Text = _currentMapName;
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Вызывается при изменении знака объекта карты.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void btnSaveSign_Click(object sender, EventArgs e)
        {
            if (_currentEditObject == null || !_btnSaveSign.Enabled)
                return;
            try
            {
                _grpMap.Focus();
                _txtSign.Text = _txtSign.Text.Trim();
                if (string.IsNullOrEmpty(_txtSign.Text) || _txtSign.Text == StrSign)
                    _txtSign.Text = "0";
                _currentEditObject.Sign = new SignValue(Convert.ToInt32(_txtSign.Text, CultureInfo.InvariantCulture));
                MapCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
            finally
            {
                try
                {
                    _btnDebug.Enabled = true;
                    _btnTest.Enabled = true;
                    _btnSaveSign.Enabled = false;
                    _currentEditObject = null;
                    _pbMap.Enabled = true;
                    _btnSave.Enabled = true;
                    _btnLoad.Enabled = true;
                    _currentPainter.Paint();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message);
                }
            }
        }

        /// <summary>
        /// Вызывается при нажатии кнопки загрузки новой карты.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_btnLoad.Enabled)
                    return;
                _grpMap.Focus();
                if (_dialogLoad.ShowDialog(this) == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(_dialogLoad.FileName, FileMode.Open))
                    {
                        _currentMap = new Map(fs);
                    }
                    _currentPainter = new Painter(_pbMap, _currentMap);
                    _currentPainter.ObjectSelect += MapSelection;
                    _currentMapName = Path.GetFileName(_dialogLoad.FileName);
                    Text = _currentMapName;
                }
                MapCount();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, StrMapLoad);
            }
        }

        /// <summary>
        /// Вызывается при редактировании объекта карты или вводе знака для проведения теста или отладки.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void txtSign_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_txtSign.Enabled)
                    return;
                _currentgrChange.Clear((new SignValue(Convert.ToInt32(_txtSign.Text))).ValueColor);
            }
            catch { }
            finally
            {
                _pbChange.Refresh();
            }
        }

        /// <summary>
        /// Вызывается при нажатии клавиши над полем ввода знака.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Агрументы события.</param>
        private void txtSign_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((Keys)e.KeyChar == Keys.Enter || (Keys)e.KeyChar == Keys.Tab || (Keys)e.KeyChar == Keys.Pause || (Keys)e.KeyChar == Keys.XButton1 || e.KeyChar == 15)
                e.Handled = true;
        }

        /// <summary>
        /// Вызывается, когда пользователь работает с клавиатуры, для корректного отображения выделенного объекта.
        /// </summary>
        /// <returns>Возвращает значение true в случае, когда фокус был установлен на окно карты, false - когда фокус находится на окне ввода знака.</returns>
        bool KeyboardReady()
        {
            if (_txtSign.Focused)
                return false;
            _grpMap.Focus();
            return true;
        }

        /// <summary>
        /// Вызывается при нажатии какой-либо клавиши клавиатуры, находясь на основной форме приложения.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void MainFrm_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F10:
                        _grpMap.Focus();
                        _currentDebuggerState = true;
                        _currentDebuggerWait = true;
                        break;
                    case Keys.F5:
                        _grpMap.Focus();
                        if (e.Control)
                        {
                            if (_currentThreadTest == null)
                                btnTest_Click(null, null);
                        }
                        else
                            btnDebug_Click(null, null);
                        break;
                    case Keys.Up:
                        if (!KeyboardReady())
                            break;
                        _currentPainter.ObjectScrY--;
                        _currentPainter.ObjectScrX = _currentPainter.ObjectScrX;
                        break;
                    case Keys.Down:
                        if (!KeyboardReady())
                            break;
                        _currentPainter.ObjectScrY++;
                        _currentPainter.ObjectScrX = _currentPainter.ObjectScrX;
                        break;
                    case Keys.Left:
                        if (!KeyboardReady())
                            break;
                        _currentPainter.ObjectScrX--;
                        _currentPainter.ObjectScrY = _currentPainter.ObjectScrY;
                        break;
                    case Keys.Right:
                        if (!KeyboardReady())
                            break;
                        _currentPainter.ObjectScrX++;
                        _currentPainter.ObjectScrY = _currentPainter.ObjectScrY;
                        break;
                    case Keys.Delete:
                        if (!e.Control && !_txtSign.Focused)
                            mapPictureBox_MouseUp(null, new MouseEventArgs(MouseButtons.Right, 1, _currentPainter.CurrentX, _currentPainter.CurrentY, 0));
                        break;
                    case Keys.S:
                        if (e.Control)
                            btnSave_Click(null, null);
                        break;
                    case Keys.O:
                        if (e.Control)
                            btnLoad_Click(null, null);
                        break;
                    case Keys.E:
                        if (e.Control)
                            pbChange_Click(null, null);
                        break;
                    case Keys.C:
                        if (e.Control && !_txtSign.Focused)
                            MapObjectCopy(false);
                        break;
                    case Keys.X:
                        if (e.Control && !_txtSign.Focused && _btnDebug.Text != StrDebugStop)
                            MapObjectCopy(true);
                        break;
                    case Keys.V:
                        if (e.Control && !_txtSign.Focused && _btnDebug.Text != StrDebugStop)
                            MapObjectPaste();
                        break;
                    case Keys.Enter:
                        btnSaveSign_Click(null, null);
                        break;
                    case Keys.Space:
                        OnSpace();
                        break;
                    case Keys.Tab:
                        OnTab();
                        break;
                }
                _currentPainter.Paint();
                e.SuppressKeyPress = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Вызывается при нажатии клавиши "Таб".
        /// </summary>
        void OnTab()
        {
            if (_grpMap.Focused)
            {
                _txtSign.Focus();
                mapPictureBox_MouseUp(null, new MouseEventArgs(MouseButtons.Left, 1, _currentPainter.CurrentX, _currentPainter.CurrentY, 0));
                return;
            }
            if (_txtSign.Focused)
                _grpMap.Focus();
        }

        /// <summary>
        /// Вызывается при нажатии клавиши "Пробел" над картой.
        /// </summary>
        void OnSpace()
        {
            if (!_currentPainter.IsDebugMode)
            {
                _txtSign.Focus();
                return;
            }
            int cx = (_currentPainter.DrawDebugFind.ObjectX * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine;
            int cy = (_currentPainter.DrawDebugFind.ObjectY * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine;
            if ((_currentPainter.CurrentX != cx || _currentPainter.CurrentY != cy) && _currentSpaceHint == SpaceHints.FindObject)
            {
                _currentPainter.CurrentX = cx;
                _currentPainter.CurrentY = cy;
                _currentSpaceHint = SpaceHints.SignStart;
                return;
            }
            int tx = (_currentPainter.DrawDebugFind.ObjectX * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine;
            int ty = (_currentPainter.DrawDebugFind.ObjectY * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine + Painter.ConstEnterOrCreatedY;
            if ((_currentPainter.CurrentX != tx || _currentPainter.CurrentY != ty) && _currentSpaceHint == SpaceHints.SignStart)
            {
                _currentPainter.CurrentX = tx;
                _currentPainter.CurrentY = ty;
                _currentSpaceHint = SpaceHints.SignNew;
                return;
            }
            int rx = (_currentPainter.DrawDebugFind.ObjectX * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine + Painter.ConstCreatedX;
            int ry = (_currentPainter.DrawDebugFind.ObjectY * Painter.ConstSize) + Painter.BetweenEls + Painter.AroundLine + Painter.ConstEnterOrCreatedY;
            if ((_currentPainter.CurrentX != rx || _currentPainter.CurrentY != ry) && _currentSpaceHint == SpaceHints.SignNew)
            {
                _currentPainter.CurrentX = rx;
                _currentPainter.CurrentY = ry;
                _currentSpaceHint = SpaceHints.FindObject;
            }
        }

        /// <summary>
        /// Указание координат указателя мыши на карте.
        /// Отображение текущего состояния карты, если окно ввода знака не имеет фокус.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void mapPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (_txtSign.Focused)
                    return;
                _currentPainter.CurrentX = e.X;
                _currentPainter.CurrentY = e.Y;
                _currentPainter.Paint();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Сохранение карты в формат XML.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_btnSave.Enabled)
                    return;
                _grpMap.Focus();
                if (_currentMap.Count <= 0)
                {
                    MessageBox.Show(this, StrMapEmpty);
                    return;
                }
                if (_dialogSave.ShowDialog(this) == DialogResult.OK)
                {
                    using (FileStream fs = new FileStream(_dialogSave.FileName, FileMode.Create))
                    {
                        _currentMap.ToStream(fs);
                    }
                    _currentMapName = Path.GetFileName(_dialogSave.FileName);
                    Text = _currentMapName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, StrMapSave);
            }
        }

        /// <summary>
        /// Обрабатывает событие получения фокуса окном для ввода знака.
        /// Если при получении фокуса ввода значение в поле знака было по умолчанию, то необходимо очистить поле.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void txtSign_Enter(object sender, EventArgs e)
        {
            if (_txtSign.Text == StrSign)
                _txtSign.Text = string.Empty;
        }

        /// <summary>
        /// Создаёт окно с возможностью выбора знака редактируемого объекта карты.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void pbChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_pbChange.Enabled)
                    return;
                if (_dialogColor.ShowDialog(this) != DialogResult.OK)
                    return;
                _currentgrChange.Clear(_dialogColor.Color);
                _txtSign.Text = (_dialogColor.Color.ToArgb() & 0x00FFFFFF).ToString();
                _txtSign.Focus();
                _pbChange.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Вызывается при запуске тестового прогона.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_btnTest.Enabled)
                    return;
                if (_currentMap.Count <= 0)
                {
                    MessageBox.Show(this, StrMapEmpty);
                    return;
                }
                _txtSign.Text = _txtSign.Text.Trim();
                if (string.IsNullOrEmpty(_txtSign.Text) || _txtSign.Text == StrSign)
                    _txtSign.Text = "0";
                ThreadTesting(new object[] { new SignValue(Convert.ToInt32(_txtSign.Text)), false });
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        /// <summary>
        /// Вызывается при запуске тестирования с участием отладчика.
        /// </summary>
        /// <param name="sender">Вызывающий объект.</param>
        /// <param name="e">Аргументы события.</param>
        private void btnDebug_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_btnDebug.Enabled)
                    return;
                MapCount();
                if (_currentMap.Count <= 0)
                {
                    MessageBox.Show(this, StrMapEmpty);
                    return;
                }
                if (_currentThreadTest != null)
                {
                    _currentDebuggerState = false;
                    _currentDebuggerWait = true;
                    return;
                }
                _txtSign.Text = _txtSign.Text.Trim();
                if (string.IsNullOrEmpty(_txtSign.Text) || _txtSign.Text == StrSign)
                    _txtSign.Text = "0";
                _currentThreadTest = new Thread(new ParameterizedThreadStart(ThreadTesting))
                {
                    IsBackground = true,
                    Priority = ThreadPriority.AboveNormal,
                    Name = "TestThread"
                };
                _currentThreadTest.Start(new object[] { new SignValue(Convert.ToInt32(_txtSign.Text)), true });
                _btnDebug.Text = StrDebugStop;
                _btnTest.Enabled = false;
            }
            catch (Exception ex)
            {
                _currentThreadTest = null;
                _btnDebug.Text = StrDebug;
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}