   M S B A M L   `   `   `  ����	  hotel    hotel.ManageRoom   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5        N� Title$  Manage Room��5	      .��5      �� $
��Auto=�6   $��Disabled=�66   .��5   
   � $��1��6   $��20q�6   
������� � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Quản lý phòng��6   $��24(�6)   $
��Bold"�67   $��0,0,0,20q�6I   $��0��6[   �� 5      -   5        $��Tạo mới phòng��5      $	��120��6-   $��40��69   
��� � �
�������$��Right=�5   *   $��0,0,0,10q�6F   $��0��6X   &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ RoomDataGrid��5         AutoGenerateColumns$ False��6)      
IsReadOnly$
 True��6E   $	��500��6W   $��0,0,0,10q�6d   $��1��6�      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5       ,  &System.Windows.Controls.DataGridColumn   Header$ Tên phòng��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ * 6U   1  +System.Windows.Controls.DataGridBoundColumn   Binding 68   �� *
Roomname+6U     5!      $ Loại phòng��6%   $ * 6V    69   �� *
RoomType+6V     5"      $ Diện tích��6%   $ * 6U    68   �� *
Capacity+6U     5#      $ 
Giá/đêm��6%   $ * 6X    66   �� *PricePerNight+6X     5$      $ Trạng thái��6%   $ * 6T    69   �� *Status+6T     5%      $ Tầng��6%   $ * 6M    63   �� *Floor+6M     5&      $ Số giường��6%   $ * 6V    68   �� *	BedNumber+6V     5'      $ 
Chi tiết��6%   $ * 6W    67   �� *Description+6W   5)      5*      5+      5-      