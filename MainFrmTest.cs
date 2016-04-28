using DynamicProcessor;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Imitator
{
    /// <summary>
    /// Класс главной формы приложения.
    /// </summary>
    public partial class MainFrm
    {
        /// <summary>
        /// Текущий поток, выполняющий тест.
        /// </summary>
        Thread _currentThreadTest;
        /// <summary>
        /// Используется для ожидания реакции пользователя на отладочное событие.
        /// Значение true означает, что сигнал от пользователя подан и требуется реакция на него.
        /// Значение false означает необходимость ожидания сигнала от пользователя.
        /// События нельзя использовать, т.к. они могут вызвать баг платформы NET в Windows XP SP2,
        /// из-за которого приложение аварийно завершит работу.
        /// </summary>
        volatile bool _currentDebuggerWait;
        /// <summary>
        /// Задаёт процессору реакцию на отладочное событие. Значение true - продолжить, false - остановиться.
        /// </summary>
        bool _currentDebuggerState;

        /// <summary>
        /// Выполняет тест для заданного объекта. Предназначена для работы в другом потоке.
        /// </summary>
        /// <param name="signAndDebugMode">Массив типа object[] со стартовым знаком и флагом (bool), обозначающим, включить отладочный режим или нет.</param>
        void ThreadTesting(object signAndDebugMode)
        {
            try
            {
                SignValue sign; bool debugMode;
                {
                    object[] masArgs = (object[])signAndDebugMode;
                    sign = (SignValue)masArgs[0];
                    debugMode = (bool)masArgs[1];
                }
                Processor _currentCommandExecutor = new Processor(_currentMap);
                if (debugMode)
                    _currentCommandExecutor.ProcDebugObject = DebugObject;
                Stopwatch totalSw = new Stopwatch();
                totalSw.Start();
                SignValue? cursign = _currentCommandExecutor.Run(sign);
                totalSw.Stop();
                if (cursign == null)
                    return;
                Invoke((Action)delegate()
                    {
                        MapCount();
                        _grpMap.Text += string.Format(CultureInfo.CurrentCulture, " {0} теста: {1:N2} {2}", StrTime, totalSw.Elapsed.TotalMilliseconds, StrMilliseconds);
                        _txtSign.Focus();
                        _txtSign.Text = cursign.ToString();
                    });
            }
            catch (Exception ex)
            {
                try
                {
                    Invoke((Action)(() => MessageBox.Show(this, ex.Message)));
                }
                catch { }
            }
            finally
            {
                _currentPainter.DrawDebugNew = null;
                _currentPainter.DrawDebugStart = null;
                _currentPainter.DrawDebugFind = null;
                _currentThreadTest = null;
                _currentMap.ClearDiscount();
                Invoke((Action)(() =>
                {
                    try
                    {
                        _btnSave.Enabled = true;
                        _lblDiscount.Text = StrNotDiscounted;
                        _lblDiscountTotal.Text = StrNotDiscounted;
                        Text = _currentMapName;
                        _btnLoad.Enabled = true;
                        _btnTest.Enabled = true;
                        _btnDebug.Enabled = true;
                        _txtSign.Enabled = true;
                        _pbChange.Enabled = true;
                        _btnDebug.Text = StrDebug;
                        _currentPainter.Paint();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                }));
            }
        }

        /// <summary>
        /// Вызывается при отладочной остановке.
        /// </summary>
        /// <param name="objNew">Порождаемый объект.</param>
        /// <param name="objStart">Стартовый объект.</param>
        /// <param name="objFind">Найденный объект.</param>
        /// <param name="count">Количество пройденных объектов.</param>
        /// <returns>Возвращает определённое пользователем состояние отладчика.</returns>
        bool DebugObject(SignValue objNew, SignValue objStart, MapObject objFind, int count)
        {
            try
            {
                Invoke((Action)(() =>
                {
                    try
                    {
                        _currentPainter.DrawDebugNew = objNew;
                        _currentPainter.DrawDebugStart = objStart;
                        _currentPainter.DrawDebugFind = objFind;
                        _btnSave.Enabled = false;
                        _btnLoad.Enabled = false;
                        _lblDiscountTotal.Text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}", count, _currentMap.Count);
                        _txtSign.Text = objStart.ToString();
                        _currentPainter.Paint();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, ex.Message);
                    }
                }));
                _currentDebuggerWait = false;
                while (!_currentDebuggerWait)
                    Thread.Sleep(50);
                _currentDebuggerWait = false;
                return _currentDebuggerState;
            }
            catch (Exception ex)
            {
                _currentDebuggerState = false;
                _currentDebuggerWait = false;
                MessageBox.Show(this, ex.Message);
                return false;
            }
        }
    }
}