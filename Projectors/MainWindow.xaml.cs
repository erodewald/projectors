using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Projectors.Annotations;
using Projectors.Properties;

namespace Projectors {
  public partial class MainWindow : Window, INotifyPropertyChanged {
    #region Fields
    public static readonly RoutedUICommand SendRemoteCommand = new RoutedUICommand();
    public static readonly RoutedUICommand SendQueryCommand = new RoutedUICommand();
    public static readonly RoutedUICommand SendSetCommand = new RoutedUICommand();
    public static readonly RoutedUICommand ConnectCommand = new RoutedUICommand();
    public static readonly RoutedUICommand DisconnectCommand = new RoutedUICommand();
    private readonly object _lock = new object();
    private InputCommand _command;
    private NetworkStream _netStream;
    private TcpClient _tcpClient;
    private string cachedConnection;
    private IPEndPoint cachedEndPoint;
    private bool isConnected;
    private string setCommandValue;
    private InputCommandCollection vivitekInputs;
    private InputCommandCollection vivitekQueries;
    private InputCommandCollection vivitekSets;
    #endregion

    public MainWindow() {
      InitializeComponent();
      Closing += (sender, args) => Settings.Default.Save();
      VivitekInputs = InputCommandCollection.GetVivitekRemoteCommands();
      VivitekSets = InputCommandCollection.GetVivitekSetCommands();
      VivitekQueries = InputCommandCollection.GetVivitekQueryCommands();
    }
    
    public async Task Connect(string input) {
      string address = null;
      int port = 0;
      IPAddress ipAddress;

      var s = input.Split(':');
      if (s.Length == 1)
        address = s[0];
      if (s.Length == 2) {
        address = s[0];
        port = Convert.ToInt16(s[1]);
      }
      if (address == null) {
        LogMessage(string.Format("{0} is not a valid input", input), false);
        return;
      }

      if (port == 0) {
        LogMessage(string.Format("A port is required"), false);
        return;
      }

      IPAddress.TryParse(address, out ipAddress);
      if (ipAddress == null) {
        LogMessage(string.Format("{0} is not a valid input", address), false);
        return;
      }
      cachedConnection = string.Format("{0}:{1}", address, port);
      cachedEndPoint = new IPEndPoint(ipAddress, port);
      IsConnected = await ConnectInternal(cachedEndPoint);
    }

    #region CommandBinding Handlers    
    
    private void ConnectCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = !IsConnected;
    }

    private async void ConnectCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      string address = e.Parameter is string
                               ? (string) e.Parameter
                               : null;
      if (address == null) return;
      LogMessage("Opening socket connection.");
      await Connect(address);
      OnPropertyChanged("IsClientConnected");
    }

    private void DisconnectCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = IsConnected;
    }

    private void DisconnectCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      Disconnect();
    }

    private void SendCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      e.CanExecute = IsConnected;
    }

    private void SendSetCommandCanExecute(object sender, CanExecuteRoutedEventArgs e) {
      var command = e.Parameter as InputCommand;
      if (command == null || SetCommandValue == null || string.IsNullOrWhiteSpace(SetCommandValue)) return;

      int setValue = -1;
      var isValidInput = int.TryParse(SetCommandValue, out setValue);
      if (isValidInput
          && command.AvailableRange != null
          && command.AvailableRange.Any()
          && command.AvailableRange.Contains(setValue)
          && IsConnected)
        e.CanExecute = true;
    }

    private async void SendRemoteCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (e.Parameter is InputCommand) await SendMessageAsync((InputCommand) e.Parameter);
    }

    private async void SendQueryCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (e.Parameter is InputCommand)
        await SendMessageAsync((InputCommand) e.Parameter);
    }

    private async void SendSetCommandExecuted(object sender, ExecutedRoutedEventArgs e) {
      if (e.Parameter is InputCommand) {
        await SendMessageAsync((InputCommand) e.Parameter, SetCommandValue);
        SetCommandValue = string.Empty;
      }
    }
    #endregion

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      var handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    #region Public Properties
    public InputCommandCollection VivitekInputs {
      get { return vivitekInputs; }
      set {
        if (Equals(value, vivitekInputs)) return;
        vivitekInputs = value;
        OnPropertyChanged();
      }
    }

    public InputCommandCollection VivitekSets {
      get { return vivitekSets; }
      set {
        if (Equals(value, vivitekSets)) return;
        vivitekSets = value;
        OnPropertyChanged();
      }
    }

    public InputCommandCollection VivitekQueries {
      get { return vivitekQueries; }
      set {
        if (Equals(value, vivitekQueries)) return;
        vivitekQueries = value;
        OnPropertyChanged();
      }
    }

    public string SetCommandValue {
      get { return setCommandValue; }
      set {
        if (value == setCommandValue) return;
        setCommandValue = value;
        OnPropertyChanged();
      }
    }

    public bool IsConnected {
      get { return isConnected; }
      set {
        if (value.Equals(isConnected)) return;
        isConnected = value;
        OnPropertyChanged();
      }
    }

    public bool IsClientConnected {
      get {
        try {
          lock (_lock) {
            var result = false;
            if (_tcpClient != null && _tcpClient.Client != null && _tcpClient.Client.Connected) {
              if (_tcpClient.Client.Poll(-1, SelectMode.SelectWrite))
                result = true;
              else if (_tcpClient.Client.Poll(-1, SelectMode.SelectRead))
                result = true;
              else if (_tcpClient.Client.Poll(-1, SelectMode.SelectError))
                result = true;
            }
            // Coerce commands to reevaluate
            CommandManager.InvalidateRequerySuggested();
            return result;
          }
        } catch {
          CommandManager.InvalidateRequerySuggested();
          return false;
        }
      }
    }
    #endregion

    #region Private Methods
    private void EnsureFileExists(string path) {
      if (!File.Exists(path))
        File.Create(path);
    }

   private void Disconnect() {
      try {
        if (IsClientConnected) {
          // Shut down client to break any async operations
          _tcpClient.Client.Shutdown(SocketShutdown.Both);
          _tcpClient.Close();
          LogMessage("Shutting down socket connection");
          OnPropertyChanged("IsClientConnected");
          IsConnected = IsClientConnected;
          if (!IsClientConnected)
            LogMessage("Socket connection closed.");
        }
      } catch {
        LogMessage("Exception thrown trying to disconnect");
      }
    }

    private void LogMessage(string text, bool append = true, [CallerMemberName] string callerName = "") {
      text = text.Trim();
      text = text.Replace("\0", string.Empty);
      var formatted = string.Format("[{0}] [Caller={1}]: {2}\n", DateTime.Now, callerName, text);
      if (append) Output.Text += formatted;
      else Output.Text = formatted;
    }

    private async Task<bool> ConnectInternal(IPEndPoint endPoint, InputCommand command = null, string value = null) {
      try {
        // Delay minimum 1ms time between commands
        await Task.Delay(1);

        // Create and configure TcpClient 
        _tcpClient = new TcpClient {ReceiveTimeout = 1000, SendTimeout = 1000, ReceiveBufferSize = 1024, SendBufferSize = 1024};
        _command = command;
        // Make connection asynchronously 
        await _tcpClient.ConnectAsync(endPoint.Address, endPoint.Port);
        _netStream = _tcpClient.GetStream();
        ThreadPool.QueueUserWorkItem(o => TcpListener());

        // Send a message asynchronously
        return await SendMessageAsync(value: value);
      } catch (SocketException ex) {
        LogMessage(ex.Message);
        return false;
      } catch (IOException ioException) {
        LogMessage(ioException.Message);
        return false;
      }
    }

    private async void TcpListener() {
      while (true) {
        if (IsClientConnected && _netStream.DataAvailable) {
          var bufferSize = _tcpClient.ReceiveBufferSize;
          var incoming = new byte[bufferSize];
          await _netStream.ReadAsync(incoming, 0, _tcpClient.ReceiveBufferSize);

          // Encode bytes to ASCII 
          var returnData = Encoding.ASCII.GetString(incoming);
          Dispatcher.Invoke(() => LogMessage(returnData.StartsWith(VivitekResponses.AckResponse)
                                                     ? string.Format("[{0}{1}] {2}", _command.IsSetter
                                                                                             ? "SET "
                                                                                             : string.Empty, _command.Name
                                                                     , "OK")
                                                     : string.Format("[GET {0}] {1}", _command.Name, returnData)));
          CommandManager.InvalidateRequerySuggested();
        }
        await Task.Delay(50);
      }
    }

    private async Task<bool> SendMessageAsync(InputCommand command = null, string value = null) {
      if (!IsClientConnected) {
        OnPropertyChanged("IsConnected");
        OnPropertyChanged("IsClientConnected");
        if (!await AttemptReconnection(command, value)) {
          LogMessage("Could not reconnect, try again later.");
          return false;
        }
      }
      try {
        if (command != null)
          _command = command;

        // Assign a generic status query if nothing is requested
        if (_command == null)
          _command = new InputCommand(VivitekCommands.StatusCommand, "Alive?");
        var rawCommand = _command.Command;

        if (!string.IsNullOrWhiteSpace(value))
          rawCommand = string.Format(_command.Command, value);

        // Encode ASCII to bytes 
        var outgoing = Encoding.ASCII.GetBytes(rawCommand);
        await _netStream.WriteAsync(outgoing, 0, outgoing.Length);

        // If the command is a setter, we want to query immediately to get the latest
        if (_command.HasDependentQuery) {
          await Task.Delay(100);
          outgoing = Encoding.ASCII.GetBytes(_command.DependentCommand);
          await _netStream.WriteAsync(outgoing, 0, outgoing.Length);
        }
      } catch (IOException e) {
        LogMessage(e.Message);
        OnPropertyChanged("IsClientConnected");
        return false;
      }
      return true;
    }

    private async Task<bool> AttemptReconnection(InputCommand command = null, string value = null) {
      for (int i = 1; i <= 10; i++) {
        LogMessage(string.Format("Attempt {0} to reconnect...", i));
        if (await ConnectInternal(cachedEndPoint, command, value)) {
          LogMessage("Successfully reconnected.");
          await SendMessageAsync(command, value);
          return true;
        }
        LogMessage("Reconnection failed. Retrying...");
#if DEBUG
        await Task.Delay(1000);
#else
        await Task.Delay(5000);
#endif
      }
      return false;
    }
    #endregion
  }
}
