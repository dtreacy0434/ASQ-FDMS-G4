   M S B A M L   `   `   `  ����P  LASQ_AircraftTelemetry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:AircraftTelemetryAircraftTelemetry  5      "    AircraftTelemetry.SearchView   5      W SPresentationCore, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=5.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:AircraftTelemetry     5      .��5
      � 
���   �  � ColumnDefinitions  5   
   �� 5      $��50 �6   �� 5      �� 5      �� 5      $��50 �6    � RowDefinitions 5   
   �� 5      $��65 �6   �� 5      $��35 �6   �� 5      .+�5   
   �� -   5        $��Search��5      $
��Left=�6"   $��1��6D   $��0��6T   
�������$��30��6{   $	��300��6�   $	��top=�6�   $��15q�6�   $��Center=�6�   6�   �� 5   
   $��2��6   $��0��6$   
�������$��30��6K   $
��3000��6W   $	��top=�6d   $��15q�6|   $��Center=�6�   �� 5   
   -   5        $��LiveTimeLabel��5      $��%TIME%��6%   $��2��66   $��16(�6F   
���   �$��Center=�6g   $��Center=�6�   �� 5   
   -   5         ��Name$ FlightTailNo��5      $��1��6*   $��1��67   
�������$��Center=�6a   $
��Left=�6|   $��5q�6�   $��20(�6�   �� 5"   
   $��1��6   $��2��6!   $��Real-Time Mode��61   
�������$��Center=�6a   $
��Left=�6|   $��5q�6�   $��20(�6�   �� 5#   
   $��1��5$      $��2��5%      $��Enabled��5&      $��Right=�5(      $��Center=�5)      $	��150��5*      $��0,0,10,0q�5+      ��5'      �� *GotoRealTimeCommand+5+      � 5-   
   $��2��6   $��1��6   $��2��6,     5.      �� 5/       51      �� 52      �� 53      $��25 �6    .+�56      �� $��1q�6   
�������$��0��6D   $��0��6T    �� CornerRadius$ 5��6a   &   System.Windows.Controls.DataGrid  58      -   5          Name$ FlightDataGrid��58         
IsReadOnly$
 True��6/   $��15(�6A   
���   �   AutoGenerateColumns$ False��6b      Columns 59      0  *System.Windows.Controls.DataGridTextColumn  5:      ,  &System.Windows.Controls.DataGridColumn   Header$ 	Date/Time��6)   1  +System.Windows.Controls.DataGridBoundColumn	   Binding	 6<   �� *StorageTime+  5;      $ Accel-X��6)   	 6:   �� *X+  5<      $ Accel-Y��6)   	 6:   �� *Y+  5=      $ Accel-Z��6)   	 6:   �� *Z+  5>      $ Weight��6)   	 69   �� *Weight+  5?      $ Altitude��6)   	 6;   �� *
Altitude+  5@      $ Pitch��6)   	 68   �� *Pitch+  5A      $
 Bank��6)   	 67   �� *Bank+5C      5D      � 5G   
   -   5        
 �Name$
 SearchInputBox��5G      $y�	Collapsed=�6'   $��2��6>   $��1��6K   $��2��6[   .+�5H      � 
���   � J� Opacity$	 0.5Q�6&   $��2��64   $��1��6A   $��2��6Q   �� 5I      $	��250��5J      
�������$��1q�5L      
�������$��Center=�5N      $��Center=�5O      ._�5P      �� .+�5Q      �� $��5q�6    $)��#Enter Aircraft Tail # (e.g. C-FGAX)��6+   
���   �$
��Bold"�6i   $��Cambria)�6{   $��20(�6�   �� 5R      -   5         ��Name$ InputTextBox��5R   q   $	��150��6   $��30��6-   $��Center=�69   $��Center=�6V   6q   �� 5S       �� Orientation$ 
Horizontal=�6!   $��Center=�6:   .+�5T      �� -   5         ��Name$ 	btnSearch��5T   !   $��5q�64   $��Search��6?   
�������$��20(�6c   6q   �� 5U      -   5        $ btnCancelSearch��5U   !   $��5q�6:   $��Cancel��6E   
�������$��20(�6i   6w   5V      �� 5W      -	   5        $ txtError��5W       $y�Hidden=�62   5X      5Y      5Z      5[      5\      