# WPF-MultiLanguage-Sample

This is a complete example that implements a WPF application with dynamic multilingual. You can learn how to write a good Windows desktop application through this example. The example might be a little complicated for beginners, so it's important to read the comments carefully to understand the code.

This example uses MVVM, so it's more suitable for the current mainstream development. And you can also learn a lot about MVVM from it. The program is developed with WPF, but its main execution can also be used in UWP and so on with a little modification.

## Building and Running

1. Make sure you have installed .NET 5 or later SDK.
2. Clone the repository into your local machine.
3. And you can build the project by running the following way:
    - Run `dotnet run` directly in the `MultiLanguage` folder, or
    - Open Visual Studio Code in the root folder of the repository, and press `F5` to test the project, or
    - Open `MultiLanguage.sln` in Visual Studio or Rider, and press `F5` to build the project.

## Structure of the Project

`MultiLanguage` folder in the root of the repository is the most important part of the project. It contains the following folders and files:

- `Common` Folder: contains the common classes and interfaces.
- `Resources` Folder: contains the resources, especially the multilingual resources.
- `App.xaml` and `App.xaml.cs`: the main application file, and the main entry point of the application.
- `AssemblyInfo.cs`: the file that contains the assembly information.
- `MainWindow.xaml` and `MainWindow.xaml.cs`: the main window of the application, which will be called by `App.xaml` as the main window.
- `MainWindowViewModel.cs`: the view model of the main window.
- `MultiLanguage.csproj`: the project definition file.

### UI

This project is started with a basic blank WPF project. First, we need to determine our requirements. Obviously, we need a basic UI to implement the language switching. Our first thought would be to use a `ComboBox` listing our available languages and a `Button` to apply our selection.

So let's check out our main window UI definition in the `MainWindow.xaml` file:

```xml
<Window ...>
  <Grid>
    ...
    <!--Right-->
    <StackPanel Grid.Column="1">
      <TextBlock ... />
      <!--Bind ViewModel source as ComboBoxItem (Source) and SelectedItem (Source).-->
      <ComboBox ... />
      <!--Bind Command which will call method to apply language change.-->
      <Button ... />
    </StackPanel>
  </Grid>
</Window>
```

The `StackPanel` holds the elements we need in order, where the `TextBlock` just prompts the user to `"Choose Your Language:"`. In order to make the effect of our language change more intuitive, we divided the window into two parts, the left part is used to display the effect, and the right part is used to change the language. The above content is the part on the right.

Next is the left part, which has a `TextBlock` and `Image` to show the changes to the text and image respectively:

```xml
...
<Grid>
  ...
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*" />
    <ColumnDefinition Width="*" />
  </Grid.ColumnDefinitions>
  <!--Left-->
  <StackPanel Grid.Column="0">
    <!--Bind Application.Resources.ResourceDictionary.MergedDictionaries in App.xaml.-->
    <TextBlock ... />
    <Image ... />
  </StackPanel>
  ...
</Grid>
...
```

We assign the default `Style` to the `StackPanel` to separate two panel with `Margin`:

```xml
...
<Grid.Resources>
  <!--StackPanel Default Style-->
  <Style TargetType="StackPanel">
    <Setter Property="Margin" Value="10" />
  </Style>
</Grid.Resources>
...
```

Finally, there is a trick about multilingual programs. We can set the attribute `SizeToContent` to `"WidthAndHeight"` in Window to make the default window size specified by the content instead of a fixed value, which can avoid that the content cannot be fully displayed because the window is too small for some languages. This attribute can also be used with `MinHeight` and `MinWidth`:

```xml
<Window ... SizeToContent="WidthAndHeight" MinHeight="450" MinWidth="800">
  ...
</Window>
```

## How will our language assets be perceived by the UI

In fact, we have just seen this way of storing Source. In `MainWindow.xaml`, We use `Grid.Resources` to store the default style, those resources are shared and applied to all child objects of the `Grid`. To make multilingual resources accessible to the whole application, we choose to store them in the `Application.Resources` as `ResourceDictionary` in `App.xaml` and `ResourceDictionary.MergedDictionaries`, which is initially empty , can help us combine multiple dictionaries for processing:

```xml
<Application ...>
  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <!--Language source will be added here.-->
        <!--<d:ResourceDictionary Source="\Resource\Language\en-US.xaml" />-->
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

As a result, we can use `DynamicResource` in `MainWindow.xaml` to obtain resources. `StaticResource` is also supported, but it will not be updated after initialization. If you expect your application should change language after restart, you should use `StaticResource` instead of `DynamicResource`. The syntax of both is similar: `{DynamicResource ResourceKey=KEY}`, `KEY` is the key of the resource you need in the `ResourceDictionary`. Example in `MainWindow.xaml`:

```xml
...
<TextBlock Text="{DynamicResource ResourceKey=Display_Hello_World}" FontSize="30" />
<Image Source="{DynamicResource ResourceKey=Main_Image}" MaxWidth="256" />
...
```

After these objects are bound to the `ResourceKey`, the content to be displayed will be determined through the `Key` of the `MergedDictionaries` element.

## Update `MergedDictionaries`

These content related to interface update can be combined as a `ViewModel`, we named it `MainWindowViewModel`. `ViewModel` needs to implement a very simple interface `INotifyPropertyChanged`. There is only one `PropertyChanged` event in this interface. This event will be used to notify the value of the property that there is a change:

```cs
public interface INotifyPropertyChanged {
    event PropertyChangedEventHandler? PropertyChanged;
}
```

We can provide some help methods for the implementation of the interface by creating an abstract class `BindableBase`.

`SetProperty` will update the value for the property, and call the `OnPropertyChanged` method after updating the value. The return value of this method is whether the value of the property has changed or not. The `CallerMemberName` attribute before the parameter `propertyName` can help programmers avoid duplicating the name of the property:

```cs
protected virtual Boolean SetProperty<T>(ref T storage, T value, [CallerMemberName] String? propertyName = null) {
    if (Object.Equals(storage, value)) {
        return false;
    }
    storage = value;
    this.OnPropertyChanged(propertyName);
    return true;
}
```

`OnPropertyChanged` simply invokes the `PropertyChanged` event to notify which property has changed. This method also uses the `CallerMemberName` attribute:

```cs
protected virtual void OnPropertyChanged([CallerMemberName] String? propertyName = null) {
    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
```

As mentioned earlier, `MergedDictionaries` will initially be empty. So we need a way to update it, which is the `UpdateApplicationLanguage` method.

The language determined by the user will be stored in `SelectedLanguage`, and then `LoadLanguageResourceDictionary` will convert the user's choice into a dictionary `langResource`.

```cs
...
ResourceDictionary? langResource = LoadLanguageResourceDictionary(this.SelectedLanguage) ??
                                   LoadLanguageResourceDictionary();
...
```

After that, clear the `MergedDictionaries` and add a new dictionary `langResource` to complete the language update.

```cs
...
Application.Current.Resources.MergedDictionaries.Clear();
...
Application.Current.Resources.MergedDictionaries.Add(langResource);
```

The content of the `LoadLanguageResourceDictionary` method is relatively simple, it just reads the resource according to the incoming language code, then converts the resource into a `ResourceDictionary` and returns it.

```cs
...
var langUri = new Uri($@"\Resource\Language\{lang}.xaml", UriKind.Relative);
return Application.LoadComponent(langUri) as ResourceDictionary;
...
```

Of course, precautions are also made here for situations such as resource non-existence. When the setting fails, the language of the interface will fall back to `ApplicationDefaultLanguage`.

## Get language menu

Next is the time to call the `UpdateApplicationLanguage` method. It is easy to think that there are two opportunities:

- When the program is initialized.
- The moment the user presses the Apply button after selecting a language.

At this time, you will find that we need to maintain a list to list all languages. A clumsy way is to go to `MainWindow.xaml` to manually type these options. And this will separate the update logic. So we need a mechanism to automatically maintain a list in `MainWindowViewModel`, and then the UI can display all available languages through the list.

In order to improve scalability, we use `ObservableCollection` here. This kind of list can be updated dynamically, and the update can be notified to the UI, so that the UI is also updated when its content is updated. In addition, we built a new class `LanguageTypeInfo` to store some information about available languages:

```cs
public class LanguageTypeInfo {

    public LanguageTypeInfo(String tag, String content) {
        this.Tag = tag;
        this.Content = content;
    }

    public String Tag { get; }

    public String Content { get; }
}
```

`Tag` represents the code name of the language, while `Content` represents the name to be displayed to the user. Separating the two helps users discern the language, and expressions like `"English (US)"` are far friendlier than `"en-US"`.

This list will be stored in the `LanguageCollection` property. In the construction function of `MainWindowViewModel`, the content of `LanguageCollection` is updated with a hard-coded method, which can be replaced with some more flexible methods to update in actual production. For example, if you want users to install custom language resources by themselves, the logic here can be replaced with automatic detection from the path:

```cs
// Load language optional item;
this._languageCollection = new ObservableCollection<LanguageTypeInfo> {
    new("en-US", "English (US)"),
    new("zh-TW", "繁體中文（台灣）"),
    new("zh-CN", "简体中文（中国）")
};
```

Similarly, we also need another field `SelectedLanguage` to store the language selected by the user, which has been mentioned before. The type of this field is `String`, because it is enough to store the `Tag` of the language selected by the user here. Maybe you will notice that the `set` of this property is a bit special. `SetProperty` is to notify the program that the property has changed, which is required by the `INotifyPropertyChanged` interface. And `RaiseCanExecuteChanged` is related to some user experience enhancements later, you can ignore it here:

```cs
public String? SelectedLanguage {
    get => this._selectedLanguage;
    set {
        if (this.SetProperty(ref this._selectedLanguage, value)) {
            ...
        }
    }
}
```

Then it's time to call the `UpdateApplicationLanguage` method. When the program is initialized, we first use a simple trick to optimize the user experience. The program will first read the language of the system, then set the user's options to that language, and finally update the program language:

```cs
// Initialize with the system language,
// if it fails, use the default language.
var cultureName = System.Globalization.CultureInfo.CurrentCulture.Name;
// this.SelectedLanguage = LoadLanguageResourceDictionary(cultureName) is null ? "en-US" : cultureName;
this.SelectedLanguage = this._languageCollection.Any(item => item.Tag == cultureName) ? cultureName : ApplicationDefaultLanguage;
this.UpdateApplicationLanguage();
```

Of course, you can also further expand the logic, such as inserting storage and reading functions for the user's preferred language here, and so on.

On the other hand, the button press event needs to be implemented with `ICommand` in Data Binding. You can understand that the `Execute` method is the method to be executed, and `CanExecute` will determine whether the button is available, and `CanExecuteChanged` is a method used to transfer the state. Change event, this event is related to the state change of CanExecute:

```cs
public interface ICommand {
    event EventHandler? CanExecuteChanged;
    Boolean CanExecute(Object? parameter);
    void Execute(Object? parameter);
}
```

This interface is actually very simple, and programmer have to manually write a lot of codes to complete the logical requirements of the interface when instantiating methods. So we need some wrappers for this interface to make it easier to use. Wrapping this interface is beyond the scope of this article, so we skip that part. As a result, we get two classes, `DelegateCommandBase` and `DelegateCommand`. The former one completes some basic logic, and users can write `ICommand` objects more easily through this abstract class. The latter is a relatively more complete implementation. `ICommand` objects with simple enough logic can be easily implemented through this class. You only need to provide the appropriate `CanExecute` and `Execute` methods. These two methods are passed in through delegation. These two classes also provide the `RaiseCanExecuteChanged` method to notify that the state of `CanExecute` will change, and the latter also provides a suitable construction function to facilitate us to quickly generate suitable instances:

```cs
public DelegateCommand(Action executeMethod, Func<Boolean> canExecuteMethod)
    : base((o) => executeMethod(), (o) => canExecuteMethod()) {
    if (executeMethod is null || canExecuteMethod is null) {
        throw new ArgumentNullException(nameof(executeMethod));
    }
}
```

So our design goal for this `ICommand` will be to update the language of the program after the user presses the button. Here's a little trick to enhance the user experience: If the current language of the program is the selected language, the button will become unavailable. Because it is pointless to replace the current language with the current language again.

We can design such a method, and this method is the `CanExecute`. Obviously, we only need to check whether the current language in `MergedDictionaries` is consistent with `SelectedLanguage`, which is easy to do:

```cs
var dictionaryResources = Application.Current.Resources;
if (dictionaryResources["Language_Code"] is String lang) {
    return SelectedLanguage != lang;
}
return false;
```

The next thing to focus on is the timing of usability changes, and there are also two:

- When the button is pressed to update.
- When the user's choice of language is changed.

So the `Execute` method would look like this:

```cs
this.UpdateApplicationLanguage();
this.ApplyLanguage.RaiseCanExecuteChanged();
```

This also explains the structure of the `SelectedLanguage` method. When the value of `SelectedLanguage` is changed, the state of `CanExecute` will be notified that there will be a change.

```cs
if (this.SetProperty(ref this._selectedLanguage, value)) {
    this.ApplyLanguage.RaiseCanExecuteChanged();
}
```

## Connect UI and ViewModel

<!-- Binding的默認DataContext會是DataContext所以我們首先在MainWindow.xaml.cs中建立MainWindowViewModel，然後將其指派至DataContext。另外我們同時也在類別中建立了ViewModel屬性，這是為了在一些多頁面專案中公開 -->
The default `DataContext` of `Binding` will be `DataContext`, so in `MainWindow.xaml`, we will implement `MainWindowViewModel` in `Window.DataContext`, which can avoid lengthy `DataContext` assignment:

```xml
<Window.DataContext>
  <local:MainWindowViewModel />
</Window.DataContext>
```

The binding of the button is relatively simple, we only need to bind the `ApplyLanguage` to the `Command`, and what the `Command` accepts is an `ICommand`.

```xml
<!--Bind Command which will call method to apply language change.-->
<Button ... Command="{Binding Path=ApplyLanguage}" />
```

`ComboBox` is quite special, it has two sets of data that need to be bound. `ItemsSource` will bind a list to display all optional items. `SelectedValue` needs to bind a specific value, which is the user's selected item. We first bind the `LanguageCollection` to the `ItemsSource`. At this point, we can notice that the item type in the latter list is `LanguageTypeInfo` instead of a `String` that can be displayed. The `Content` property in `LanguageTypeInfo` provides a `String` for display, so we can use `DisplayMemberPath` to specify the `Content` in `LanguageTypeInfo` as the display. Similarly, `SelectedLanguage` should accept `String` objects, which correspond to `Tags` in `LanguageTypeInfo`, and `SelectedValuePath` can help us do this:

```xml
<!--Bind ViewModel source as ComboBoxItem (Source) and SelectedItem (Source).-->
<ComboBox ItemsSource="{Binding Path=LanguageCollection}"
          DisplayMemberPath="Content"
          SelectedValue="{Binding Path=SelectedLanguage}"
          SelectedValuePath="Tag" />
```
