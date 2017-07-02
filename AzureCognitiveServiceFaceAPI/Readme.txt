We are going to follow the preceding steps to create the Face API application

1. Create the Face API in portal, and get the key, end point URI if you select location as Southeast Asia
2. Create a WPF application
3. Drag Image, Button controls from the Toolbox, and apply the needed settings.
4. Write the code for loading the image to our image control
5. Write the code for detecting the faces count
6. Write the code for drawing the rectangles in faces


#3 Drag Image, Button controls from the Toolbox, and apply the needed settings

<Window x:Class="AzureCognitiveServiceFaceAPI.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:local="clr-namespace:AzureCognitiveServiceFaceAPI"
        mc:Ignorable="d"
        Title="MainWindow" Height="350" Width="430">
    <Grid>
        <Image Stretch="UniformToFill" x:Name="FaceImage" HorizontalAlignment="Left" Margin="0,0,0,30"/>
        <Button x:Name="BtnUpload" VerticalAlignment="Bottom" Content="Upload the image" Margin="20,5" Height="20" Click="BtnUpload_Click"/>
    </Grid>
</Window>

#4 Write the code for loading the image to our image control

var openFrame = new Microsoft.Win32.OpenFileDialog { Filter = "JPEG Image(*.jpg)|*.jpg" };
            var result = openFrame.ShowDialog(this);
            if (!(bool)result)
                return;
            var filePath = openFrame.FileName;
            var fileUri = new Uri(filePath);
            var bitMapSource = new BitmapImage();
            bitMapSource.BeginInit();
            bitMapSource.CacheOption = BitmapCacheOption.None;
            bitMapSource.UriSource = fileUri;
            bitMapSource.EndInit();
            FaceImage.Source = bitMapSource;

#5 Write the code for detecting the faces count

 private readonly IFaceServiceClient _faceServiceClient = new FaceServiceClient("key", "end point");
 private async Task<FaceRectangle[]> DetectTheFaces(string filePath)
        {
            try
            {
                using (var imgStream = File.OpenRead(filePath))
                {
                    var faces = await _faceServiceClient.DetectAsync(imgStream);
                    var faceRectangles = faces.Select(face => face.FaceRectangle);
                    return faceRectangles.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

			Title = "Detecting....";
            FaceRectangle[] facesFound = await DetectTheFaces(filePath);
            Title = $"Found {facesFound.Length} faces";


#6 Write the code for drawing the rectangles in faces
