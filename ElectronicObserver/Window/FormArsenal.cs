﻿using ElectronicObserver.Data;
using ElectronicObserver.Utility.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace ElectronicObserver.Window {

	public partial class FormArsenal : DockContent {

		private class TableArsenalControl {

			public Label ShipName;
			public Label CompletionTime;


			public TableArsenalControl( FormArsenal parent ) {

				#region Initialize

				ShipName = new Label();
				ShipName.Text = "???";
				ShipName.Anchor = AnchorStyles.Left;
				ShipName.Font = parent.Font;
				ShipName.ForeColor = parent.ForeColor;
				ShipName.TextAlign = ContentAlignment.MiddleLeft;
				ShipName.Padding = new Padding( 0, 1, 0, 1 );
				ShipName.Margin = new Padding( 2, 0, 2, 0 );
				ShipName.MaximumSize = new Size( 60, 20 );
				ShipName.AutoEllipsis = true;
				ShipName.AutoSize = true;
				ShipName.Visible = true;

				CompletionTime = new Label();
				CompletionTime.Text = "";
				CompletionTime.Anchor = AnchorStyles.Left;
				CompletionTime.Font = parent.Font;
				CompletionTime.ForeColor = parent.ForeColor;
				CompletionTime.Tag = null;
				CompletionTime.TextAlign = ContentAlignment.MiddleLeft;
				CompletionTime.Padding = new Padding( 0, 1, 0, 1 );
				CompletionTime.Margin = new Padding( 2, 0, 2, 0 );
				CompletionTime.MinimumSize = new Size( 60, 10 );
				CompletionTime.AutoSize = true;
				CompletionTime.Visible = true;
				
				#endregion

			}


			public TableArsenalControl( FormArsenal parent, TableLayoutPanel table, int row )
				: this( parent ) {

				AddToTable( table, row );
			}


			public void AddToTable( TableLayoutPanel table, int row ) {

				table.Controls.Add( ShipName, 0, row );
				table.Controls.Add( CompletionTime, 1, row );

				#region set RowStyle
				RowStyle rs = new RowStyle( SizeType.Absolute, 21 );

				if ( table.RowStyles.Count > row )
					table.RowStyles[row] = rs;
				else
					while ( table.RowStyles.Count <= row )
						table.RowStyles.Add( rs );
				#endregion

			}


			public void Update( int arsenalID ) {

				KCDatabase db = KCDatabase.Instance;
				ArsenalData arsenal = db.Arsenals[arsenalID];

				if ( arsenal == null || arsenal.State == -1 ) {
					//locked
					ShipName.Text = "";
					CompletionTime.Text = "";
					CompletionTime.Tag = null;
				
				} else if ( arsenal.State == 0 ) {
					//empty
					ShipName.Text = "----";
					CompletionTime.Text = "";
					CompletionTime.Tag = null;

				} else if ( arsenal.State == 2 ) {
					//building
					ShipName.Text = db.MasterShips[arsenal.ShipID].Name;
					CompletionTime.Text = DateConverter.ToTimeRemainString( arsenal.CompletionTime );
					CompletionTime.Tag = arsenal.CompletionTime;

				} else if ( arsenal.State == 3 ) {
					//complete!
					ShipName.Text = db.MasterShips[arsenal.ShipID].Name;
					CompletionTime.Text = "完成!";
					CompletionTime.Tag = null;

				}

			}


			public void Refresh( int arsenalID ) {

				if ( CompletionTime.Tag != null ) {
					CompletionTime.Text = DateConverter.ToTimeRemainString( (DateTime)CompletionTime.Tag );
				}
			}

		}


		private TableArsenalControl[] ControlArsenal;


		public FormArsenal( FormMain parent ) {
			InitializeComponent();

			parent.UpdateTimerTick += parent_UpdateTimerTick;

			TableArsenal.SuspendLayout();
			ControlArsenal = new TableArsenalControl[4];
			for ( int i = 0; i < ControlArsenal.Length; i++ ) {
				ControlArsenal[i] = new TableArsenalControl( this, TableArsenal, i );
			}
			TableArsenal.ResumeLayout();
		}


		
		private void FormArsenal_Load( object sender, EventArgs e ) {

			KCDatabase Database = KCDatabase.Instance;

			Database.ArsenalsUpdated += ( DatabaseUpdatedEventArgs e1 ) => Invoke( new KCDatabase.DatabaseUpdatedEventHandler( Database_ArsenalsUpdated ), e1 );
		
		}


		void Database_ArsenalsUpdated( DatabaseUpdatedEventArgs e ) {

			TableArsenal.SuspendLayout();
			for ( int i = 0; i < ControlArsenal.Length; i++ )
				ControlArsenal[i].Update( i + 1 );
			TableArsenal.ResumeLayout();

		}

		void parent_UpdateTimerTick( object sender, EventArgs e ) {

			TableArsenal.SuspendLayout();
			for ( int i = 0; i < ControlArsenal.Length; i++ )
				ControlArsenal[i].Refresh( i + 1 );
			TableArsenal.ResumeLayout();

		}


		private void TableArsenal_CellPaint( object sender, TableLayoutCellPaintEventArgs e ) {
			e.Graphics.DrawLine( Pens.Silver, e.CellBounds.X, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1 );
		}




	}

}