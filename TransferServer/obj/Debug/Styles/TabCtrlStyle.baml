   M S B A M L   `   `   `  ����W  SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   CheckedState  �� 
StartPoint �� EndPoint � Color � Offset  UnCheckedState  Gobal_Tab_Style �� 
TargetType �� Property �� Value �� 
TargetType-   'System.Windows.Input.KeyboardNavigation    TabNavigation	 � RowDefinitions'  !System.Windows.VisualStateManager
   VisualStateGroups%  System.Windows.VisualStateGroup   States   System.Windows.VisualState   
Storyboard �� 
TargetName �� TargetProperty8   2System.Windows.Media.Animation.EasingColorKeyFrame �� KeyTime �� Value ��Name K� ZIndex K� IsItemsHost    TabIndex ��Name �� CornerRadius    DirectionalNavigation ��Name �Name � ColumnDefinitions � Cursor �� Triggers P� Property P� Value �� 
TargetName�� 5      D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml      5      %]  &	        &	 d     &	 �     '�� �    7  7   �� 5      $	  0,0&�6/   $	 0,1&�6@   .��5   
   � $ White��6   $ 1Q�6%   5      �� 5      $	  0,0&�61   $	 0,1&�6B   .��5   
   � $ #eeeeee��6   $ 1Q�6'   5      �� 5	      " ��6$   .�5
   
   ��  �@�OverridesDefaultStyle . 62   �� 5   
    �@J�SnapsToDevicePixels . 60   �� 5   
    � �� 5      �� 5      " ��6"   .R�5      � $ Local=�6   	 5      �� 5      $
��Auto �6,   �� 5      $��* �6,   
 5        5      CommonStates��. 5   "     Disabled��. 5   &   �� .��5   *   �� $ Border��6G   $2 ,(Border.BorderBrush).(SolidColorBrush.Color)�6f   .g�5   .     $ 0��6B   $ 	#FFAAAAAA��6N   5   +   5   '   5   #   5      .+�5      �� $ HeaderPanel��6#   $��0��68   $ 1��6E   $��0,0,0,-1q�6V    . $ 1��6{   $��Hidden=�6�   $��300000��6�   $��33��6�   �� 50      $ Border��6!   $��1��61   $��1q�6>   $ 2��6R   $ Local=�6c   
�������$ 	Contained=�51   !   $ 2��6V   ���#eeeeee._�59      �� $ PART_SelectedContentHost��6/   $��1q�6Q   $��SelectedContent��6\   5:      5;      5<      5>      5?      �� 5@      " ��6   .�5A   
   ��  � �� 5B      �� 5C      " ��6"   .R�5D      � .+�5E      �� $��tabItemBorder��6!   $��1,1,1,0q�66   ���#eeeeee8��  $��29��6�   $��0,0,2,-1q�6�   $ 1,1,0,0��6�   ._�5F      � $ tabItemPanel��6#    5G   "   �� 5H   &   $��* �67   �� 5I   &   $��24 �67   .+�5K   "   �� $ contentSite��6C   $��0��63   $��Header��6X   ��. 6o   �� 5L   "   $��1��6(   .��5M   &   �� $'��!/Resources/Images/title/close.png��6,   $
 Hand��6W   $��16��6e   $��16��6p   $��0,1,0,0q�6|   5N   #   5O      5P      5Q       5R      P� 5S       �@��
IsSelected . . �5T      ��  �@K�ZIndex$	 100��6=   �� 5U      $ tabItemBorder��6%    � ��$ 1,1,1,0q�6[   �� 5V      $ tabItemBorder��6%   8   � ��6V   5W      5Y      5[      5\      5]      