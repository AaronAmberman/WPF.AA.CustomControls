# WPF.AA.CustomControls
A custom control library for WPF with custom look-less control templates.

# Controls
- ColorPicker
- ColorSlider
- GridSplitterPopupExpander
- NumericUpDown
- PopupExpander
- RoundableButton
- RoundableToggleButton
- ShowHidePasswordBox
- WatermarkTextBox
- ZoomableListBox

Please check out the [wiki](https://github.com/AaronAmberman/WPF.AA.CustomControls/wiki) for more information on each control. 

# ColorPicker
A color picker control that can be utilized in WPF applications. Uses HSV color space math rather than sampling bitmaps for color accuracy. There is a reason for this...please see [wiki](https://github.com/AaronAmberman/WPF.AA.CustomControls/wiki/ColorPicker) for more information.

![image](https://i.imgur.com/I0rN2sL.png)

# GridSplitterPopupExpander Example
Behaves like the AvalonDock side auto popup window, except that it cannot be ripped out or pinned.

Check out my [MainWindow](https://github.com/AaronAmberman/VTOLVR-MissionAssistant/blob/main/VTOLVR-MissionAssistant/VTOLVR-MissionAssistant/MainWindow.xaml) in my [VTOL VR Mission Assitant](https://github.com/AaronAmberman/VTOLVR-MissionAssistant) for an example.

![image](https://user-images.githubusercontent.com/23512394/224828476-8134783a-138d-4081-b0fe-84d67f8db06b.png)

```
<GridSplitterPopupExpander ExpandDirection="Up" PopupContentMinimumHeight="100"
                           PopupBoundsElement="{Binding ElementName=innerMainGrid}"
                           Background="#FF2F2F2F" Foreground="#FFFFFFFF" 
                           GridSplitterBackground="#FF007BFF"
                           Header="{Binding Translations.Warnings, FallbackValue=Warnings}">
```

Fairly simple to implement. ***PopupBoundsElement*** should be some kind of root Grid or Border. This is used for positioning and sizing data. This property is required!

### Translation Curiosity
Curious about what I am doing for translations and how I can bind to translated strings? Check out my WPF transations API, [WPF.Translations](https://github.com/AaronAmberman/WPF.Translations).

# NumericUpDown
The NumericUpDown control is really easy to use but a couple of things to cover. First, the ValueType property...

The ValueType property specifies the type of data for the Value property. You can set Integer, Decimal or Double. This value is used behind the scenes to convert the values to the appropriate numeric type so comparisons and assignments can occur properly.

Secondly, the ValueFormat property...

The ValueFormat property is the [StringFormat](https://learn.microsoft.com/en-us/dotnet/api/system.windows.data.bindingbase.stringformat?view=windowsdesktop-7.0) used in the binding to the Value property.

```
<cc:NumericUpDown ValueType="Double" ValueFormat="N2" />
<cc:NumericUpDown /> <!-- default ValueType is Integer -->
```

As you can see it is very easy to use. However you can setup a UX that is confusing by not properly utilizing the two aforementioned properties. Meaning, you can set the ValueType to Integer then set the ValueFormat to N2 (or whatever) and get 2 decimal places on your display. As a developer you can set that up but don't. If you are using the N# format then use Decimal or Double. It is also recommended to use Decimal over Double is most situations. The control will not try to add a decimal if one should be there for display, such as in the case of double or decimal data types. The problem is the developers to manage by specifying a ValueFormat.

### NumericUpDownDecimal and NumericUpDownDouble
In WPF.AA.CustomControls.DataTemplateControls you will find two variants for the NumericUpDown control. The reason is because of DataTemplate boundaries. Google it if you are unfamiliar. These controls do not alter the behavior in any way, they just change the default ValueType to decimal and double. This is the entirity of the code...

```
using System.Windows;

namespace WPF.AA.CustomControls.DataTemplateControls
{
    public class NumericUpDownDecimal : NumericUpDown
    {
        static NumericUpDownDecimal()
        {
            ValueTypeProperty.OverrideMetadata(typeof(NumericUpDownDecimal), new PropertyMetadata(NumericUpDownType.Decimal));
        }
    }
}
```

As you can see all we do is override the ValueType property, everything else will be the same...including the UI. You can use these variants anywhere but they are required in DataTemplates if you want something other than Integer. Assignments and bindings will fail because of the coercion mechanism, it will try to use integers to do the math instead of decimals or doubles.
