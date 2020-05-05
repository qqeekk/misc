<!--
    This is the simple PHP script to show connected hosts based on openvpn status file.
-->

<?php
define('STATUS_FILE', './openvpn-status.log');

error_reporting(E_ALL);

function human_filesize($bytes, $decimals = 2)
{
    $size = array('B','kB','MB','GB','TB','PB','EB','ZB','YB');
    $factor = floor((strlen($bytes) - 1) / 3);
    return sprintf("%.{$decimals}f", $bytes / pow(1024, $factor)) . ' ' . @$size[$factor];
}
?>

<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.1.0/css/bootstrap.min.css" integrity="sha384-9gVQ4dYFwwWSjIDZnLEWnxCjeSWFphJiwGPXr1jddIhOegiu1FwO5qRGvFXOdJZ4" crossorigin="anonymous">
    <title>OpenVPN Status</title>
</head>

<body>
    <div class="container-fluid">
        <?php $file = fopen(STATUS_FILE, 'r') or die('Cannot open file.'); ?>
        <table class="table table-sm">
            <thead>
                <tr>
                    <th scope="col">name</th>
                    <th scope="col">host</th>
                    <th scope="col">inet</th>
                    <th scope="col">inet6</th>
                    <th scope="col">rx bytes</th>
                    <th scope="col">tx bytes</th>
                    <th scope="col">connected since</th>
                    <th scope="col">username</th>
                    <th scope="col">client id</th>
                    <th scope="col">peer id</th>
                </tr>
            </thead>
            <tbody>
            <?php
                $title = '';
                $totalrx = 0;
                $totaltx = 0;
                $totalclient = 0;
                while (!feof($file)) {
                    $line = fgets($file);
                    $arr = explode(',', $line);
                    echo '<tr>';
                    if (count($arr) > 0 && $arr[0] == 'CLIENT_LIST') {
                        $totalclient++;
                        $totalrx += (int)$arr[5];
                        $totaltx += (int)$arr[6];
                        echo '<td>' . $arr[1] . '</td>';
                        echo '<td><pre>' . $arr[2] . '</pre></td>';
                        echo '<td><pre>' . $arr[3] . '</pre></td>';
                        echo '<td><pre>' . $arr[4] . '</pre></td>';
                        echo '<td>' . human_filesize($arr[5]) . '</td>';
                        echo '<td>' . human_filesize($arr[6]) . '</td>';
                        echo '<td>' . $arr[7] . '</td>';
                        echo '<td>' . $arr[9] . '</td>';
                        echo '<td>' . $arr[10] . '</td>';
                        echo '<td>' . $arr[11] . '</td>';
                    }
                    if (count($arr) > 0 && $arr[0] == 'TITLE') {
                        $title = $arr[1];
                    }
                }
                echo '</tr>';
                echo '<tr>';
                echo '<td>TOTAL ' . $totalclient . '</td>';
                echo '<td></td>';
                echo '<td></td>';
                echo '<td></td>';
                echo '<td>' . human_filesize($totalrx) . '</td>';
                echo '<td>' . human_filesize($totaltx) . '</td>';
                echo '<td></td>';
                echo '<td></td>';
                echo '<td></td>';
                echo '<td></td>';
                echo '</tr>';
                fclose($file);
            ?>
            </tbody>
        </table>
        <footer class="text-muted text-right text-small">
            <?php echo $title ?>
        </footer>
    </div>
</body>

</html>
