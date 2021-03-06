﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DebugProtocol;
using WeifenLuo.WinFormsUI.Docking;

namespace RosDBG
{
    [DebugControl, BuildAtStartup, Obsolete("Replaced by StatefulRegisterView")]
    public partial class RegisterView : ToolWindow, IUseDebugConnection
    {
        bool mGridEnabled;
        Registers mRegisters;
        DebugConnection mConnection;

        public RegisterView()
        {
            InitializeComponent();
            RegisterGrid.SelectedObject = new Registers();
        }

        public void SetDebugConnection(DebugConnection conn)
        {
            mConnection = conn;
            mConnection.DebugRegisterChangeEvent += DebugRegisterChangeEvent;
            mConnection.DebugRunningChangeEvent += DebugRunningChangeEvent;
            mConnection.DebugConnectionModeChangedEvent += DebugConnectionModeChangedEvent;
            if (!mConnection.Running)
            {
                mConnection.Debugger.GetRegisterUpdate();
                mConnection.Debugger.GetProcesses();
            }
        }

        void DebugConnectionModeChangedEvent(object sender, DebugConnectionModeChangedEventArgs args)
        {
            if (mConnection.ConnectionMode == DebugConnection.Mode.ClosedMode)
                Invoke(Delegate.CreateDelegate(typeof(NoParamsDelegate), this, "ClearRegs"));
        }

        void DebugRunningChangeEvent(object sender, DebugRunningChangeEventArgs args)
        {
            mGridEnabled = !args.Running;
            Invoke(Delegate.CreateDelegate(typeof(NoParamsDelegate), this, "UpdateGridEnabled"));
        }

        void DebugRegisterChangeEvent(object sender, DebugRegisterChangeEventArgs args)
        {
            mRegisters = args.Registers;
            Invoke(Delegate.CreateDelegate(typeof(NoParamsDelegate), this, "UpdateGrid"));
        }

        void ClearRegs()
        {
            if (mRegisters != null)
                mRegisters.Clear();
            UpdateGrid();
        }

        void UpdateGridEnabled()
        {
            RegisterGrid.Enabled = mGridEnabled;
        }

        void UpdateGrid()
        {
            RegisterGrid.SelectedObject = null;
            RegisterGrid.SelectedObject = mRegisters;
            RegisterGrid.Refresh();
        }

    }
}
