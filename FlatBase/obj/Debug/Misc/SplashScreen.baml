   M S B A M L   `   `   `  ����C  ?FlatBase, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:FlatBase.MiscFlatBase.Misc  5   	   O,clr-namespace:FlatBase.Misc.SplashScreenMiscFlatBase.Misc.SplashScreenMisc  5   	        FlatBase.Misc.SplashScreen   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:FlatBase.Misc     5   	   ISplashScreenMisc,clr-namespace:FlatBase.Misc.SplashScreenMisc     5   	     � Title$  SplashScreen��5	   	   $	��450��6   $	��800��6+    � WindowStyle$
 None=�67    � Topmost .  � WindowStartupLocation$ CenterScreen=�6Y    � 
ResizeMode$ NoResize=�6~    � ShowInTaskbar .  ��5
      �� 5   
    �� ImageSource$ splashBG.png��6   .��5      �  � ColumnDefinitions 5   
   �� 5      $
��400* �6   �� 5      $
��151* �6   �� 5      $
��241* �6   .+�5   
   �� $��__GJDE��6   $
��Left=�6!   $��	10,10,0,0q�6<   $	��Top=�6O   $��84��6g   $	��644��6s   ���	#DDA5BDC0$��48(�6�   $��3��6�   �� 5   
   
���   �$��0,0,2,0q�6%   $
��Left=�6?   $	��278��6Z   $��
29,141,0,0q�6g   $	��Top=�6{   $	��372��6�   ���	#72000000._�5      ��  �� CanVerticallyScroll . .+�5      �� $��Stretch=�6   $��100px��6=   $	��278��6N   .��5      �� $��Stretch=�6!   .+�5       9� $��Recent��6#   $��Stretch=�63   ���	#00F02B2B���	#FFD3C9C9	 9� ExpandDirection$
	 Down=�6}   $��Stretch=�6�   
 9� 
IsExpanded
 . ��5!      �� 5"   "   -   5        $��projectList��5"   -   5$   #   5&      5'      5(      5)      5*      �� 5+   
   $��Open Recent��6   $
��Left=�6&   $��	10,94,0,0q�6A   $	��Top=�6T   $��42��6l   $	��261��6x   ���	#DDA5BDC0$��24(�6�   �� 5,   
   $��Get Started��6   $
��Left=�6&   $��
129,94,0,0q�6A   $	��Top=�6U   $��42��6m   $	��130��6y   ���	#DDA5BDC0$��24(�6�   $��2��6�   $��1��6�   �� 5-   
   $
��Left=�6   $	��292��60   $��129,141,0,0q�6=   $	��Top=�6R   $	��232��6j   $��2��6v   $��1��6�    � 	Resources 5.       �� 
TargetType �� Property �� Value��%X   '��       �� 5/      " ��6   .�50      ��  � ��$ 0,10,0,0q�6/   51      52      .+�54      �� -   5        $��48��54      ���	#FF4F4F4F$��64��6;   ���	#002196F3.��55      � $	��232��6   $��64��6#   .+�56      �� $��32, 0, 0, 0q�6   $��New Project��61   ���	#DDA5BDC0$��16(�6^   $
��Left=�6l   $	��Top=�6�   6�   �� 57      $��32, 20, 0, 0q�6   ���	#DDA5BDC0$��12(�6I   $
��Left=�6W   $	��Top=�6r   $��48��6�   $	��200��6�   .��6�   Create a new empty project.59      5:      5;      �� 5=      -   5        $��48��5=      ���	#FF4F4F4F$��64��6;   ���	#002196F3.��5>      � $	��232��6   $��64��6#   .+�5?      �� $��32, 0, 0, 0q�6   $��Open Project��61   ���	#DDA5BDC0$��16(�6_   $
��Left=�6m   $	��Top=�6�   6�   �� 5@      $��32, 20, 0, 0q�6   ���	#DDA5BDC0$��12(�6I   $
��Left=�6W   $	��Top=�6r   $��48��6�   $	��200��6�   .��6�   Load a project.5B      5C      5D      �� 5F      $��48��6   ���	#FF4F4F4F$��64��6;   ���	#002196F3{�.  .��5G      � $	��232��6   $��64��6#   .+�5H      �� $��32, 0, 0, 0q�6   $��Linked Project��61   ���	#DDA5BDC0$��16(�6a   $
��Left=�6o   $	��Top=�6�   6�   �� 5I      $��32, 20, 0, 0q�6   ���	#DDA5BDC0$��12(�6I   $
��Left=�6W   $	��Top=�6r   $��48��6�   $	��200��6�   .��6�   #!Create a codebase linked project.5K      5L      5M      �� 5O      ���	#DD6067E7$��Center=�6+    � Cursor$
 Hand��6H   .��6V   Visit the project on Github6s   5R      �� 5S   
   $
��Left=�6   $��96��6+   $��174,-37,-29,0q�67   $	��Top=�6N   $��96��6f   $��logoGreeble.png��6q    J� RenderTransformOrigin$ 1.041,0.124&�6�   $��2��6�   5U      5V      