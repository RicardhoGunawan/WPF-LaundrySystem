   M S B A M L   `   `   `  ����  laundry(    "Laundry.Windows.StatusUpdateWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  Update Status Pesanan��5   	   $	��300��6'   $	��450��64    � WindowStartupLocation$ CenterOwner=�5   	   .��5	      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Update Status Pesanan��6!   $��18(�5      $
��Bold"�6"   $��0,0,0,20q�64   � 5   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   �� 5      $
��Auto �6#   �� 5      $��* �6#    5      �� 5      $
��Auto �6    �� 5       $
��Auto �6    .+�5#      �� $��0��6   $��0��6%   $��No. Pesanan:��65   $
��Bold"�6I   $��0,5,10,5q�6[   �� 5$      -   5         ��Name$ OrderIdTextBlock��5$   5   $��0��6   $��1��6%   $��0,5,20,5q�6O   �� 5&      $��0��6   $��2��6%   $��
Pelanggan:��65   $
��Bold"�6G   $��0,5,10,5q�6Y   �� 5'      -   5        $ CustomerNameTextBlock��5'   5   $��0��6   $��3��6%   $	��0,5q�6T   �� 5)      $��1��6   $��0��6%   $��Status Saat Ini:��65   $
��Bold"�6M   $��0,5,10,5q�6_   �� 5*      -   5        $ CurrentStatusTextBlock��5*   5   $��1��6   $��1��6%   $��0,5,20,5q�6U   5+      � 5.   
   $��2��6   $��0,0,0,20q�6    5/      �� 50      $
��Auto �6#   �� 51      $��* �6#   .+�54      �� $��0��6   $��Status Baru:��6(   $��Center=�6<   $��0,0,10,0q�6W   �� 55      -   5         ��Name$ StatusComboBox��55   '   $��1��6   $	��0,5q�6?   56      � 59   
   $��3��6    5:      �� 5;      $
��Auto �6#   �� 5<      $��* �6#   .+�5?      �� $��0��6   $��Catatan:��6(   $	��Top=�68   $��0,5,10,0q�6P   �� 5@      -   5         ��Name$ NotesTextBox��5@   &   $��1��6    �� TextWrapping$
 Wrap=�6<    �� AcceptsReturn$
 True��6P   $��60��6e   $	��0,5q�6q   5A      �� 5D   
   $��4��6   	 �� Orientation$	 
Horizontal=�6"   $��Right=�6;   $��0,20,0,0q�6W   .+�5E      �� -   5        
 ��Name$
 UpdateButton��5E      $��Update��6+   $	��100��6W   $��0,0,10,0q�6c   �� 5F      -   5        $
 CancelButton��5F      $��Batal��6+   $	��100��6V   5G      5H      5I      