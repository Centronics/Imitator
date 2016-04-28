using DynamicProcessor;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Imitator
{
    /// <summary>
    /// Кнопки объекта карты.
    /// </summary>
    public enum Buttons
    {
        /// <summary>
        /// Нераспознана.
        /// </summary>
        NoButton,
        /// <summary>
        /// Знак.
        /// </summary>
        Sign,
        /// <summary>
        /// Порождаемый знак.
        /// </summary>
        SignCreated,
        /// <summary>
        /// Стартовый знак.
        /// </summary>
        SignEnter
    }

    /// <summary>
    /// Предназначен для отображения текущей карты на экране.
    /// </summary>
    sealed class Painter : IDisposable
    {
        /// <summary>
        /// Размер кнопки "Знак" объекта карты.
        /// </summary>
        const int ConstElem = 32;
        /// <summary>
        /// Расстояние между объектами карты.
        /// </summary>
        public const int BetweenEls = 10;
        /// <summary>
        /// Ширина в пикселях, характеризующая ширину рамки, в которую обводится пройденный процессором объект карты.
        /// </summary>
        const int Debugged = 6;
        /// <summary>
        /// Ширина линии, обводящей знак отлаживаемого объекта при наведении указателя мыши в режиме отладки.
        /// </summary>
        const int DebugSelSign = 4;
        /// <summary>
        /// Ширина линии, обводящей знак отлаживаемого объекта при отсутствии указателя мыши.
        /// </summary>
        const int DebugUnSelSign = 2;
        /// <summary>
        /// Ширина линии, обводящей стартовый или порождаемый знак отлаживаемого объекта при наведении указателя мыши.
        /// </summary>
        const int DebugSelSubSign = 3;
        /// <summary>
        /// Ширина линии при отсутствии указателя мыши над стартовым или порождаемым знаком отлаживаемого объекта.
        /// </summary>
        const int DebugUnSelCE = 1;
        /// <summary>
        /// Размер одного объекта карты с учётом расстояния между объектами и размеров линий-разделителей вокруг кнопок.
        /// </summary>
        public const int ConstSize = ConstElem + BetweenEls + AroundLine;
        /// <summary>
        /// Ширина линии вокруг кнопки (свойства или функции) объекта карты.
        /// </summary>
        public const int AroundLine = 1;
        /// <summary>
        /// Отступ вниз для получения координаты по оси Y для отображения обвода по краям стартового или порождаемого знаков.
        /// </summary>
        public const int ConstEnterOrCreatedY = ConstElem / 4;
        /// <summary>
        /// Отступ вправо для получения координаты по оси X для отображения обвода по краям порождаемого знака.
        /// </summary>
        public const int ConstCreatedX = ConstElem / 2;
        /// <summary>
        /// Цвет рамки отлаживаемого объекта.
        /// </summary>
        readonly Color DebugColor = Color.Red;
        /// <summary>
        /// Происходит при наведении указателя мыши на объект карты.
        /// </summary>
        public event Action<MapObject, Buttons, bool> ObjectSelect;
        /// <summary>
        /// PictureBox для рисования.
        /// </summary>
        readonly PictureBox _currentBox;
        /// <summary>
        /// Текущая карта.
        /// </summary>
        readonly Map _currentMap;
        /// <summary>
        /// Цвет выделения кнопки, на которую произошло наведение.
        /// </summary>
        readonly Pen _currentstSelected = new Pen(Color.Violet, AroundLine);
        /// <summary>
        /// Цвет невыделенной кнопки.
        /// </summary>
        readonly Pen _currentstNonSel = new Pen(Color.Red, AroundLine);
        /// <summary>
        /// Изображение карты.
        /// </summary>
        readonly Bitmap _currentCanvas;
        /// <summary>
        /// Поверхность для рисования.
        /// </summary>
        readonly Graphics _currentgrFront;
        /// <summary>
        /// Служит для отображения объектов карты.
        /// </summary>
        readonly Pen _workPen = new Pen(Color.Black);

        /// <summary>
        /// Получает или задаёт текущее положение указателя мыши по оси X (в пикселях).
        /// </summary>
        public int CurrentX { get; set; }
        /// <summary>
        /// Получает или задаёт текущее положение указателя мыши по оси Y (в пикселях).
        /// </summary>
        public int CurrentY { get; set; }
        /// <summary>
        /// Получает или задаёт порождаемый знак при отладке карты. При отладке отображается правее.
        /// </summary>
        public SignValue? DrawDebugNew { get; set; }
        /// <summary>
        /// Получает или задаёт стартовый знак. При отладке отображается левее.
        /// </summary>
        public SignValue? DrawDebugStart { get; set; }
        /// <summary>
        /// Получает или задаёт найденный объект. При отладке отображается на фоне.
        /// </summary>
        public MapObject DrawDebugFind { get; set; }

        /// <summary>
        /// Получает положение объекта, над которым находится указатель мыши на экране, по оси X (в объектах).
        /// </summary>
        public int ObjectScrX
        {
            get { return GetObjX(CurrentX); }
            set
            {
                if (value <= 0)
                {
                    CurrentX = BetweenEls + AroundLine;
                    return;
                }
                if (value >= Map.MaxX)
                    value = Map.MaxX - 1;
                CurrentX = value * ConstSize;
                CurrentX += BetweenEls + AroundLine;
                if (CurrentY <= 0)
                    CurrentY = BetweenEls + AroundLine;
            }
        }

        /// <summary>
        /// Получает положение объекта, над которым находится указатель мыши на экране, по оси Y (в объектах).
        /// </summary>
        public int ObjectScrY
        {
            get { return GetObjY(CurrentY); }
            set
            {
                if (value <= 0)
                {
                    CurrentY = BetweenEls + AroundLine;
                    return;
                }
                if (value >= Map.MaxY)
                    value = Map.MaxY - 1;
                CurrentY = value * ConstSize;
                CurrentY += BetweenEls + AroundLine;
                if (CurrentX <= 0)
                    CurrentX = BetweenEls + AroundLine;
            }
        }

        /// <summary>
        /// Вычисляет положение объекта с заданной координатой по оси X (в объектах).
        /// </summary>
        /// <param name="x">Координата X объекта.</param>
        /// <returns>Возвращает положение объекта с заданной координатой по оси X (в объектах).</returns>
        static int GetObjX(int x)
        {
            return (x / ConstSize);
        }

        /// <summary>
        /// Вычисляет положение объекта с заданной координатой по оси Y (в объектах).
        /// </summary>
        /// <param name="y">Координата Y объекта.</param>
        /// <returns>Возвращает положение объекта с заданной координатой по оси Y (в объектах).</returns>
        static int GetObjY(int y)
        {
            return (y / ConstSize);
        }

        /// <summary>
        /// Инициализирует экземпляр класса для отображения содержимого карты.
        /// </summary>
        /// <param name="pb">PictureBox для рисования.</param>
        /// <param name="info">Карта для отображения.</param>
        public Painter(PictureBox pb, Map info)
        {
            if ((_currentBox = pb) == null)
                throw new ArgumentException("CPainter: pb имеет недопустимое значение: null", "pb");
            if ((_currentMap = info) == null)
                throw new ArgumentException("CPainter: info имеет недопустимое значение: null", "info");
            _currentCanvas = new Bitmap(_currentBox.Width, _currentBox.Height);
            _currentgrFront = Graphics.FromImage(_currentCanvas);
            _currentBox.Image = _currentCanvas;
            Paint();
        }

        /// <summary>
        /// Получает значение, определяющее, находится вывод в режиме отладки или нет.
        /// </summary>
        public bool IsDebugMode
        {
            get
            {
                return (DrawDebugFind != null && DrawDebugNew != null && DrawDebugStart != null);
            }
        }

        /// <summary>
        /// Получает название клавиши, над которой находится указатель мыши в данный момент.
        /// </summary>
        public Buttons OnButton
        {
            get
            {
                int cx = CurrentX % ConstSize, cy = CurrentY % ConstSize;
                if (cx < BetweenEls || cx >= ConstSize || cy < BetweenEls || cy >= ConstSize)
                    return Buttons.NoButton;
                if (!IsDebugMode)
                    return Buttons.Sign;
                if (CurrentX >= ((DrawDebugFind.ObjectX * ConstSize) + ConstElem / 4) &&
                    CurrentX < ((DrawDebugFind.ObjectX * ConstSize) + (ConstSize / 2) + ConstElem / 5) &&
                    CurrentY >= ((DrawDebugFind.ObjectY * ConstSize) + ConstElem / 4) + BetweenEls &&
                    CurrentY < ((DrawDebugFind.ObjectY * ConstSize) + (ConstSize / 2) + BetweenEls + (ConstElem / 8)))
                    return Buttons.SignEnter;
                if (CurrentX >= ((DrawDebugFind.ObjectX * ConstSize) + (ConstSize / 2) + (ConstElem / 5)) &&
                    CurrentX < ((DrawDebugFind.ObjectX * ConstSize) + (ConstElem / 4) + ConstSize) &&
                    CurrentY >= ((DrawDebugFind.ObjectY * ConstSize) + (ConstElem / 4) + BetweenEls) &&
                    CurrentY < ((DrawDebugFind.ObjectY * ConstSize) + (ConstSize / 2) + BetweenEls + (ConstElem / 8)))
                    return Buttons.SignCreated;
                return Buttons.Sign;
            }
        }

        /// <summary>
        /// Выполняет перерисовку карты целиком, создавая событие наведения указателя мыши на объект карты.
        /// </summary>
        public void Paint()
        {
            _currentgrFront.Clear(Color.FromArgb(0xF0F0F0)); bool curs = false, debcurs = false;
            for (int x = 0; x < Map.MaxX; x++)
                for (int y = 0; y < Map.MaxY; y++)
                {
                    MapObject obj = _currentMap.GetObjectByXY(x, y);
                    if (obj != null)
                        if (obj.ObjectX == ObjectScrX && obj.ObjectY == ObjectScrY)
                        {
                            if (ObjectSelect != null && !debcurs)
                            {
                                debcurs = IsDebugMode;
                                bool debugged = IsDebugMode && DrawDebugFind.ObjectX == GetObjX(CurrentX) && DrawDebugFind.ObjectY == GetObjY(CurrentY);
                                switch (OnButton)
                                {
                                    case Buttons.Sign:
                                        ObjectSelect(obj, OnButton, debugged);
                                        break;
                                    case Buttons.SignCreated:
                                        ObjectSelect(new MapObject
                                        {
                                            Sign = DrawDebugNew.Value,
                                            DiscountNumber = obj.DiscountNumber,
                                            ObjectX = obj.ObjectX,
                                            ObjectY = obj.ObjectY,
                                        }, OnButton, debugged);
                                        break;
                                    case Buttons.SignEnter:
                                        ObjectSelect(new MapObject
                                        {
                                            Sign = DrawDebugStart.Value,
                                            DiscountNumber = obj.DiscountNumber,
                                            ObjectX = obj.ObjectX,
                                            ObjectY = obj.ObjectY,
                                        }, OnButton, debugged);
                                        break;
                                    case Buttons.NoButton:
                                        ObjectSelect(obj, Buttons.NoButton, false);
                                        break;
                                }
                            }
                            curs = true;
                        }
                    DrawObject(obj, (x * ConstSize) + BetweenEls, (y * ConstSize) + BetweenEls);
                }
            if (!curs)
                if (ObjectSelect != null)
                    ObjectSelect(null, Buttons.NoButton, false);
            _currentBox.Refresh();
        }

        /// <summary>
        /// Отображает заданный объект карты с указанными координатами, выделяя объект, если указатель мыши находится над ним.
        /// </summary>
        /// <param name="obj">Объект для отображения.</param>
        /// <param name="x">Координата по оси X.</param>
        /// <param name="y">Координата по оси Y.</param>
        void DrawObject(MapObject obj, int x, int y)
        {
            #region Константы
            const int t1 = ConstElem / 2, t2 = ConstElem / 4, t4 = ConstElem / 8;
            bool onThis = (CurrentX >= x) && CurrentX < (x + (ConstSize - BetweenEls)) && (CurrentY >= y) && CurrentY < (y + (ConstSize - BetweenEls));
            bool debugged = false;
            #endregion
            #region ЗНАК
            if (obj != null)
            {
                _workPen.Color = obj.Sign.ValueColor;
                _workPen.Width = t1;
                if ((obj.Sign.ValueColor.ToArgb() & 0x00FFFFFF) >= 0xCFE7D7)
                {
                    _currentgrFront.DrawEllipse(_workPen, x + t2, y + t2, t1, t1);
                    _workPen.Color = Color.FromArgb(unchecked((int)0xFFFFFFFF - obj.Sign.Value));
                    _workPen.Width = DebugUnSelSign;
                    _currentgrFront.DrawEllipse(_workPen, x, y, ConstElem, ConstElem);
                }
                else
                    _currentgrFront.DrawEllipse(_workPen, x + t2, y + t2, t1, t1);
                if (IsDebugMode && DrawDebugFind.ObjectX == GetObjX(x) && DrawDebugFind.ObjectY == GetObjY(y))
                {
                    _workPen.Color = Color.FromArgb(unchecked((int)0xFFFFFFFF - obj.Sign.Value));
                    _workPen.Width = ((OnButton == Buttons.Sign && onThis) ? DebugSelSign : DebugUnSelSign);
                    _currentgrFront.DrawEllipse(_workPen, x, y, ConstElem, ConstElem);
                }
            }
            if (IsDebugMode)
                if (DrawDebugFind.ObjectX == GetObjX(x) && DrawDebugFind.ObjectY == GetObjY(y))
                {
                    int selWidthInput = (OnButton == Buttons.SignEnter) ? DebugSelSubSign : DebugUnSelCE;
                    int selWidthCreated = (OnButton == Buttons.SignCreated) ? DebugSelSubSign : DebugUnSelCE;
                    _workPen.Color = DrawDebugFind.Sign.ValueColor;
                    _workPen.Width = t1;
                    _currentgrFront.DrawEllipse(_workPen, x + t2, y + t2, t1, t1);
                    _workPen.Color = DrawDebugStart.Value.ValueColor;
                    _workPen.Width = t2;
                    _currentgrFront.DrawEllipse(_workPen, x + t4, y + (t4 * 3), t2, t2);
                    _workPen.Color = Color.FromArgb(unchecked((int)0xFFFFFFFF - obj.Sign.Value));
                    _workPen.Width = selWidthInput;
                    _currentgrFront.DrawEllipse(_workPen, x, y + ConstEnterOrCreatedY, t1, t1);
                    _workPen.Color = DrawDebugNew.Value.ValueColor;
                    _workPen.Width = t2;
                    _currentgrFront.DrawEllipse(_workPen, x + (t4 * 5), y + (t4 * 3), t2, t2);
                    _workPen.Color = Color.FromArgb(unchecked((int)0xFFFFFFFF - obj.Sign.Value));
                    _workPen.Width = selWidthCreated;
                    _currentgrFront.DrawEllipse(_workPen, x + ConstCreatedX, y + ConstEnterOrCreatedY, t1, t1);
                    debugged = true;
                }
            _currentgrFront.DrawRectangle(_currentstNonSel, x, y, ConstElem, ConstElem);
            #endregion
            #region Пройден
            if (obj != null)
                if (obj.DiscountNumber >= 0 || debugged)
                {
                    const int around = (ConstSize - BetweenEls) + (Debugged - AroundLine);
                    const int step = Debugged / 2;
                    _workPen.Color = debugged ? DebugColor : Color.FromArgb(obj.DiscountNumber * 5, obj.DiscountNumber * 5, obj.DiscountNumber * 5);
                    _workPen.Width = step;
                    _currentgrFront.DrawRectangle(_workPen, x - step, y - step, around, around);
                }
            #endregion
            #region Наведение на объект
            if (onThis)
                _currentgrFront.DrawRectangle(_currentstSelected, x - AroundLine, y - AroundLine, ConstElem + (AroundLine * 2), ConstElem + (AroundLine * 2));
            #endregion
        }

        /// <summary>
        /// Освобождает все используемые ресурсы.
        /// </summary>
        public void Dispose()
        {
            _currentstSelected.Dispose();
            _currentstNonSel.Dispose();
            _currentgrFront.Dispose();
            _currentCanvas.Dispose();
            _workPen.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}