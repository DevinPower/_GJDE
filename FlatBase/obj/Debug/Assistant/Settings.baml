   M S B A M L   `   `   `  ����S  OMaterialDesignExtensions, Version=3.2.0.0, Culture=neutral, PublicKeyToken=nullwQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions!MaterialDesignExtensions.Controls  5   	   C ?FlatBase, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null7 clr-namespace:FlatBase.AssistantFlatBase.Assistant 5   	   !   FlatBase.Assistant.Settings   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   _mdeQclr-namespace:MaterialDesignExtensions.Controls;assembly=MaterialDesignExtensions    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   2local clr-namespace:FlatBase.Assistant     5   	     � Title$  Settings��5	   	   $	��450��6   $	��373��6'      MaterialDesignBody#���   5
   	   $��Regular"�5   	   $��13(�5   	   &   System.Windows.Media.TextOptions   TextFormattingMode$ Ideal=�5   	      TextRenderingMode$
 Auto=�5   	     MaterialDesignPaper#���  5   	     MaterialDesignFont#���  5   	    � 
ResizeMode$ NoResize=�6;   .��5      � .+�5   
   �� $
��Left=�6   $	��419��60   $	��Top=�6=   $	��365��6U   .@�5      �� $��Trello��6   .��5      � -   5        $��
trelloGrid��5   @   ���	#FFE5E5E5$��0,0,-2,0q�6.   .+�5      �� -   5         ��Name$ textURL��5       �� TextWrapping$
 Wrap=�6/   $��10,181,10.2,151.2q�6C    �� 
IsReadOnly . $��generate authurl��6p   {�.  6�   �� 5      -   5         ��Name$ buttonGetAuth��5      $��Get AuthURL��64   $
��Left=�6J   $��
10,144,0,0q�6e   $	��Top=�6y   $	��142��6�   {�.  6�   �� 5      $
��Left=�6    $��10,101,-2.8,0q�6;   $
��Wrap=�6R   $[��UAuthorize _GJDE from the AuthURL and then save the token to enable Trello integration��6f   $	��Top=�6�   $��38��6�   $	��354��6�   �� 5      -   5        $ 	textToken��5      $
��Left=�61   $��23��6L   $��
10,270,0,0q�6X   $
 Wrap=�6l   $	��Top=�6�   $	��341��6�   {�.  6�   �� 5      -   5        $ buttonApply��5      $��Apply��62   $
��Left=�6B   $��276,298,0,0q�6]   $	��Top=�6r   $��75��6�   {�.  6�   �� 5      -   5        $ textKey��5      $
��Left=�6/   $��23��6J   $��	10,10,0,0q�6V   $
 Wrap=�6i   $	��key��6}   $	��Top=�6�   $	��341��6�   �� 5      -   5        $ 	buttonKey��5      $��
Enable Key��60   $
��Left=�6E   $��	10,38,0,0q�6`   $	��Top=�6s   $	��116��6�   6�   �� 5      -   5         ��Name$ 
comboBoard��5      $
��Left=�63   $��
10,335,0,0q�6N   $	��Top=�6b   $	��341��6z   6�   5      5       5"      5$      5%      