   M S B A M L   `   `   `  ����  laundry*    $Laundry.Windows.CustomerOrdersWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	     � Title$  Riwayat Pesanan Pelanggan��5   	   $	��500��6+   $	��800��68    � WindowStartupLocation$ CenterOwner=�5   	   .��5	      � $��20q�6    � RowDefinitions 5
   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   .+�5   
   �� $��0��6   $��Riwayat Pesanan Pelanggan��6!   $��18(�5      $
��Bold"�6"   $��0,0,0,10q�64   � 5   
   $��1��6   $��0,0,0,20q�6    � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   �� 5      $
��Auto �6#   �� 5      $��* �6#    5      �� 5      $
��Auto �6    �� 5      $
��Auto �6    .+�5"      �� $��0��6   $��0��6%   $��Nama:��65   $
��Bold"�6B   $��0,5,10,5q�6T   �� 5#      -   5         ��Name$ NameTextBlock��5#   5   $��0��6   $��1��6%   $��0,5,20,5q�6L   �� 5%      $��0��6   $��2��6%   $��No. Telepon:��65   $
��Bold"�6I   $��0,5,10,5q�6[   �� 5&      -   5        $ PhoneTextBlock��5&   5   $��0��6   $��3��6%   $	��0,5q�6M   �� 5(      $��1��6   $��0��6%   $��Alamat:��65   $
��Bold"�6D   $��0,5,10,5q�6V   �� 5)      -   5        $ AddressTextBlock��5)   5   $��1��6   $��1��6%   $��0,5,20,5q�6O   �� 5+      $��1��6   $��2��6%   $��Tanggal Daftar:��65   $
��Bold"�6L   $��0,5,10,5q�6^   �� 5,      -   5        $ RegisterDateTextBlock��5,   5   $��1��6   $��3��6%   $	��0,5q�6T   5-      &   System.Windows.Controls.DataGrid  50   
   -   5          Name$ OrdersDataGrid��50       $��2��6      AutoGenerateColumns$ False��68      
IsReadOnly$
 True��6T      Columns 51      0  *System.Windows.Controls.DataGridTextColumn  52      ,  &System.Windows.Controls.DataGridColumn	   Header$	 No. Pesanan��6%   
   Width5  /System.Windows.Controls.DataGridLengthConverter$	
 100 6V   1  +System.Windows.Controls.DataGridBoundColumn   Binding 6:   �� *	OrderId+6V     53      $	 Tanggal��6%   $	
 150 6{    66   �� *	OrderDate+ �� StringFormat$ {0:dd/MM/yyyy HH:mm}��6{     54      $	 Total��6%   $	
 120 6m    64   �� *FinalAmount+$ {0:N0}��6m     55      $	 Status��6%   $	
 120 6P    65   �� *Status+6P     56      $	 
Pembayaran��6%   $	
 100 6�    69   �� *IsPaid+   BoolToStatusConverter �� 	Converter# [  6�   4  .System.Windows.Controls.DataGridTemplateColumn  57      $
	 Aksi��6)   $	
 150 67      CellTemplate 58      �� 59      .R�5:      �� -   5        $��Detail��5:   %   $��80��6i   
 � Tag 6Q   �� *	OrderId+6i   5;      5=      5?      �� 5B   
   $��3��6    �� Orientation$ 
Horizontal=�6"   $��Right=�6;   $��0,20,0,0q�6W   .+�5C      �� -   5         ��Name$ NewOrderButton��5C      $��Pesanan Baru��6-   $	��120��6a   $��0,0,10,0q�6m   �� 5D      -   5        $ CloseButton��5D      $��Tutup��6*   $	��100��6T   5E      5F      5G      