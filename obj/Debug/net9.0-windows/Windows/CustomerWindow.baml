   M S B A M L   `   `   `  ����  laundry$    Laundry.Windows.CustomerWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  Tambah Pelanggan Baru��5   	   $	��350��6'   $	��500��64    � WindowStartupLocation$ CenterOwner=�5   	    � 
ResizeMode$ NoResize=�6-   .��5	      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Tambah Pelanggan Baru��6!   $��18(�5      $
��Bold"�6"   $��0,0,0,20q�64   � 5   
   $��1��6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#    5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5"      �� $��0��6   $��0��6%   $��Nama:��65   $��Center=�6B   $��0,5,15,5q�6]   �� 5#      -   5         ��Name$ NameTextBox��5#   3   $��0��6   $��1��6#   $	��0,5q�6H   �� 5%      $��1��6   $��0��6%   $��No. Telepon:��65   $��Center=�6I   $��0,5,15,5q�6d   �� 5&      -   5        $ PhoneTextBox��5&   3   $��1��6   $��1��6#   $	��0,5q�6I   �� 5(      $��2��6   $��0��6%   $��Alamat:��65   $��Center=�6D   $��0,5,15,5q�6_   �� 5)      -   5        $ AddressTextBox��5)   3   $��2��6   $��1��6#   $	��0,5q�6K   �� 5+      $��3��6   $��0��6%   $��Email:��65   $��Center=�6C   $��0,5,15,5q�6^   �� 5,      -   5        $ EmailTextBox��5,   3   $��3��6   $��1��6#   $	��0,5q�6I   5-      �� 50   
   $��2��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,20,0,0q�6W   .+�51      �� -   5         ��Name$ 
SaveButton��51      $��Simpan��6)   $	��100��6S   $��0,0,10,0q�6_   �� 52      -   5        $ CancelButton��52      $��Batal��6+   $	��100��6V   53      54      55      