<?php
require("SSI.php");
?>
<html>
<link rel="stylesheet" type="text/css" href="http://openmeridian.org/forums/Themes/blackrain_202/css/index.css?fin20" />
<link rel="stylesheet" type="text/css" href="http://openmeridian.org/forums/Themes/blackrain_202/css/webkit.css" />
<?php
$array = ssi_boardNews(16.0, 5, null, null, 'array');

        foreach ($array as $news)
        {
                echo '
                        <table border="0" width="100%" align="center" class="ssi_table">
                                <tr>
                                        <td ><a href="',$news['comment_href'],'">', $news['subject'], '</a></td>
                                        <td width="20%" >', $news['time'], ' by ', $news['poster']['link'],'</td>
                                </tr>
                                <tr>
                                        <td >', $news['body'], '<br /><br /> Full Thread: ', $news['link'], '</td>
                                </tr>
                        </table>
                        <br />';

                if (!$news['is_last'])
                        echo '
                        <hr hr style="height:4px;width:100%;background-color:#222222" />
                        ';
        }
?>
</html>
