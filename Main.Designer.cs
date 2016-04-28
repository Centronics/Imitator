namespace Imitator
{
    partial class MainFrm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                _currentgrChange.Dispose();
                _currentbtmChange.Dispose();
                _currentPainter.Dispose();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this._dialogLoad = new System.Windows.Forms.OpenFileDialog();
            this._dialogSave = new System.Windows.Forms.SaveFileDialog();
            this._grpProp = new System.Windows.Forms.GroupBox();
            this._pbChange = new System.Windows.Forms.PictureBox();
            this._txtSign = new System.Windows.Forms.TextBox();
            this._btnSaveSign = new System.Windows.Forms.Button();
            this._btnDebug = new System.Windows.Forms.Button();
            this._btnTest = new System.Windows.Forms.Button();
            this._dialogColor = new System.Windows.Forms.ColorDialog();
            this._grpMap = new System.Windows.Forms.GroupBox();
            this._pbMap = new System.Windows.Forms.PictureBox();
            this._grpService = new System.Windows.Forms.GroupBox();
            this._btnLoad = new System.Windows.Forms.Button();
            this._btnSave = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this._lblDiscountTotal = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._lblDiscount = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this._grpProp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbChange)).BeginInit();
            this._grpMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbMap)).BeginInit();
            this._grpService.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _dialogLoad
            // 
            this._dialogLoad.DefaultExt = "xml";
            this._dialogLoad.Filter = "Карта|*.xml";
            // 
            // _dialogSave
            // 
            this._dialogSave.DefaultExt = "xml";
            this._dialogSave.Filter = "Карта|*.xml";
            // 
            // _grpProp
            // 
            this._grpProp.Controls.Add(this._pbChange);
            this._grpProp.Controls.Add(this._txtSign);
            this._grpProp.Controls.Add(this._btnSaveSign);
            this._grpProp.Location = new System.Drawing.Point(374, 81);
            this._grpProp.Name = "_grpProp";
            this._grpProp.Size = new System.Drawing.Size(123, 75);
            this._grpProp.TabIndex = 0;
            this._grpProp.TabStop = false;
            this._grpProp.Text = "Знак (Ctrl+E)";
            // 
            // _pbChange
            // 
            this._pbChange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._pbChange.Location = new System.Drawing.Point(79, 19);
            this._pbChange.Name = "_pbChange";
            this._pbChange.Size = new System.Drawing.Size(36, 22);
            this._pbChange.TabIndex = 65;
            this._pbChange.TabStop = false;
            this._pbChange.Click += new System.EventHandler(this.pbChange_Click);
            // 
            // _txtSign
            // 
            this._txtSign.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._txtSign.Location = new System.Drawing.Point(8, 19);
            this._txtSign.MaxLength = 8;
            this._txtSign.Name = "_txtSign";
            this._txtSign.Size = new System.Drawing.Size(67, 22);
            this._txtSign.TabIndex = 0;
            this._txtSign.TabStop = false;
            this._txtSign.Text = "Знак";
            this._txtSign.TextChanged += new System.EventHandler(this.txtSign_TextChanged);
            this._txtSign.Enter += new System.EventHandler(this.txtSign_Enter);
            this._txtSign.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSign_KeyPress);
            // 
            // _btnSaveSign
            // 
            this._btnSaveSign.Enabled = false;
            this._btnSaveSign.Location = new System.Drawing.Point(8, 46);
            this._btnSaveSign.Name = "_btnSaveSign";
            this._btnSaveSign.Size = new System.Drawing.Size(107, 23);
            this._btnSaveSign.TabIndex = 63;
            this._btnSaveSign.Text = "Сохранить (Enter)";
            this._btnSaveSign.UseVisualStyleBackColor = true;
            this._btnSaveSign.Click += new System.EventHandler(this.btnSaveSign_Click);
            // 
            // _btnDebug
            // 
            this._btnDebug.Location = new System.Drawing.Point(8, 42);
            this._btnDebug.Name = "_btnDebug";
            this._btnDebug.Size = new System.Drawing.Size(107, 23);
            this._btnDebug.TabIndex = 4;
            this._btnDebug.TabStop = false;
            this._btnDebug.Text = "Отладка (F5)";
            this._btnDebug.UseVisualStyleBackColor = true;
            this._btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // _btnTest
            // 
            this._btnTest.Location = new System.Drawing.Point(8, 16);
            this._btnTest.Name = "_btnTest";
            this._btnTest.Size = new System.Drawing.Size(107, 23);
            this._btnTest.TabIndex = 3;
            this._btnTest.TabStop = false;
            this._btnTest.Text = "Тест (Ctrl+F5)";
            this._btnTest.UseVisualStyleBackColor = true;
            this._btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // _grpMap
            // 
            this._grpMap.Controls.Add(this._pbMap);
            this._grpMap.Location = new System.Drawing.Point(8, 6);
            this._grpMap.Name = "_grpMap";
            this._grpMap.Size = new System.Drawing.Size(360, 252);
            this._grpMap.TabIndex = 3;
            this._grpMap.TabStop = false;
            this._grpMap.Text = "Карта";
            // 
            // _pbMap
            // 
            this._pbMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this._pbMap.Location = new System.Drawing.Point(3, 16);
            this._pbMap.Name = "_pbMap";
            this._pbMap.Size = new System.Drawing.Size(354, 233);
            this._pbMap.TabIndex = 52;
            this._pbMap.TabStop = false;
            this._pbMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseMove);
            this._pbMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.mapPictureBox_MouseUp);
            // 
            // _grpService
            // 
            this._grpService.Controls.Add(this._btnLoad);
            this._grpService.Controls.Add(this._btnSave);
            this._grpService.Location = new System.Drawing.Point(374, 6);
            this._grpService.Name = "_grpService";
            this._grpService.Size = new System.Drawing.Size(123, 76);
            this._grpService.TabIndex = 1;
            this._grpService.TabStop = false;
            this._grpService.Text = "Сервис";
            // 
            // _btnLoad
            // 
            this._btnLoad.Location = new System.Drawing.Point(8, 47);
            this._btnLoad.Name = "_btnLoad";
            this._btnLoad.Size = new System.Drawing.Size(107, 23);
            this._btnLoad.TabIndex = 2;
            this._btnLoad.TabStop = false;
            this._btnLoad.Text = "Загрузить (Ctrl+O)";
            this._btnLoad.UseVisualStyleBackColor = true;
            this._btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(8, 16);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(107, 23);
            this._btnSave.TabIndex = 1;
            this._btnSave.TabStop = false;
            this._btnSave.Text = "Сохранить (Ctrl+S)";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this._lblDiscountTotal);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this._lblDiscount);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this._btnTest);
            this.groupBox1.Controls.Add(this._btnDebug);
            this.groupBox1.Location = new System.Drawing.Point(374, 155);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(123, 103);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Тестирование";
            // 
            // _lblDiscountTotal
            // 
            this._lblDiscountTotal.AutoSize = true;
            this._lblDiscountTotal.Location = new System.Drawing.Point(65, 83);
            this._lblDiscountTotal.Name = "_lblDiscountTotal";
            this._lblDiscountTotal.Size = new System.Drawing.Size(26, 13);
            this._lblDiscountTotal.TabIndex = 9;
            this._lblDiscountTotal.Text = "Нет";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Всего:";
            // 
            // _lblDiscount
            // 
            this._lblDiscount.AutoSize = true;
            this._lblDiscount.Location = new System.Drawing.Point(65, 68);
            this._lblDiscount.Name = "_lblDiscount";
            this._lblDiscount.Size = new System.Drawing.Size(26, 13);
            this._lblDiscount.TabIndex = 7;
            this._lblDiscount.Text = "Нет";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Пройден:";
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(506, 266);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this._grpService);
            this.Controls.Add(this._grpMap);
            this.Controls.Add(this._grpProp);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainFrm_KeyUp);
            this._grpProp.ResumeLayout(false);
            this._grpProp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pbChange)).EndInit();
            this._grpMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._pbMap)).EndInit();
            this._grpService.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog _dialogLoad;
        private System.Windows.Forms.SaveFileDialog _dialogSave;
        private System.Windows.Forms.GroupBox _grpProp;
        private System.Windows.Forms.TextBox _txtSign;
        private System.Windows.Forms.Button _btnSaveSign;
        private System.Windows.Forms.PictureBox _pbChange;
        private System.Windows.Forms.ColorDialog _dialogColor;
        private System.Windows.Forms.GroupBox _grpMap;
        private System.Windows.Forms.GroupBox _grpService;
        private System.Windows.Forms.Button _btnLoad;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Button _btnTest;
        private System.Windows.Forms.Button _btnDebug;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox _pbMap;
        private System.Windows.Forms.Label _lblDiscount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _lblDiscountTotal;
        private System.Windows.Forms.Label label2;
    }
}

