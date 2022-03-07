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

The `MultiLanguage` folder in the root of the repository is the most important part of the project. It contains the following folders and files:

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
    <TextBlock Text="{DynamicResource ResourceKey=Display_Hello_World}" FontSize="30" />
    <Image Source="{DynamicResource ResourceKey=Main_Image}" MaxWidth="256" />
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
<Window ...
    SizeToContent="WidthAndHeight"
    MinHeight="450" MinWidth="800">
    ...
</Window>
```
