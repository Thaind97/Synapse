Add-Type -AssemblyName System.Drawing

$width = 256
$height = 256
$iconPath = "d:\HSG3\Synapse\Synapse.Presentation\Resources\icon.ico"
$pngPath = "d:\HSG3\Synapse\Synapse.Presentation\Resources\icon.png"

$bmp = New-Object System.Drawing.Bitmap $width, $height
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Colors
$teal = [System.Drawing.ColorTranslator]::FromHtml("#0D9488")
$brush = New-Object System.Drawing.SolidBrush $teal

# Draw Triangle Logo (synapse)
# Scale the path points (original ~80x80) to 256x256
# M40,15 L15,65 L65,65 Z M40,35 L30,55 L50,55 Z
# Scale factor approx 3
$scale = 3.2
$offsetX = 0
$offsetY = 0

$p1 = New-Object System.Drawing.PointF (40 * $scale + $offsetX), (15 * $scale + $offsetY)
$p2 = New-Object System.Drawing.PointF (15 * $scale + $offsetX), (65 * $scale + $offsetY)
$p3 = New-Object System.Drawing.PointF (65 * $scale + $offsetX), (65 * $scale + $offsetY)

$h1 = New-Object System.Drawing.PointF (40 * $scale + $offsetX), (35 * $scale + $offsetY)
$h2 = New-Object System.Drawing.PointF (30 * $scale + $offsetX), (55 * $scale + $offsetY)
$h3 = New-Object System.Drawing.PointF (50 * $scale + $offsetX), (55 * $scale + $offsetY)

$path = New-Object System.Drawing.Drawing2D.GraphicsPath
$path.AddPolygon(@($p1, $p2, $p3))
$path.AddPolygon(@($h1, $h2, $h3))

$g.FillPath($brush, $path)

# Save PNG
$bmp.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Save ICO (Simple conversion)
# For a proper multi-size ICO, we need more work, but for now let's create a single frame ICO from the PNG
# Using a temporary file approach or Icon.FromHandle (quality might vary)

$startInfo = New-Object System.Diagnostics.ProcessStartInfo
$startInfo.FileName = "powershell"
$helpers = @"
using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

public class IconHelper {
    public static void SaveAsIcon(Bitmap bmp, string path) {
        using (FileStream fs = new FileStream(path, FileMode.Create)) {
             // ICO Header
             // 0-1 reserved, 2-3 type(1=ico), 4-5 images count
             fs.WriteByte(0); fs.WriteByte(0);
             fs.WriteByte(1); fs.WriteByte(0);
             fs.WriteByte(1); fs.WriteByte(0);

             // Image Entry
             // 0 width, 1 height, 2 palette, 3 reserved, 4-5 planes, 6-7 bits per pixel, 8-11 size, 12-15 offset
             int width = bmp.Width;
             int height = bmp.Height;
             if (width >= 256) width = 0;
             if (height >= 256) height = 0;
             fs.WriteByte((byte)width);
             fs.WriteByte((byte)height);
             fs.WriteByte(0);
             fs.WriteByte(0);
             fs.WriteByte(1); fs.WriteByte(0); // planes
             fs.WriteByte(32); fs.WriteByte(0); // bpp

             // Convert to PNG in memory to get size
             MemoryStream ms = new MemoryStream();
             bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
             byte[] pngData = ms.ToArray();
             int size = pngData.Length;
             
             fs.Write(BitConverter.GetBytes(size), 0, 4);
             fs.Write(BitConverter.GetBytes(22), 0, 4); // Offset = 6+16 = 22

             // Write PNG data
             fs.Write(pngData, 0, size);
        }
    }
}
"@

Add-Type -TypeDefinition $helpers -ReferencedAssemblies System.Drawing, System.Windows.Forms

[IconHelper]::SaveAsIcon($bmp, $iconPath)

$g.Dispose()
$bmp.Dispose()

Write-Host "Icon generated successfully."
