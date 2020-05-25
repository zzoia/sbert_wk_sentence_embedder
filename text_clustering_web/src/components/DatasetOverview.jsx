import React from 'react';
import { makeStyles } from '@material-ui/core/styles';
import DatasetCardHeader from "../components/DatasetCardHeader";
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import CardActions from '@material-ui/core/CardActions';
import IconButton from '@material-ui/core/IconButton';
import Typography from '@material-ui/core/Typography';
import FavoriteIcon from '@material-ui/icons/Favorite';
import ShareIcon from '@material-ui/icons/Share';
import Button from '@material-ui/core/Button';

const useStyles = makeStyles((theme) => ({
    root: {
        width: 500,
        marginBottom: 16,
        marginTop: 16,
        paddingRight: 16,
        paddingLeft: 16
    },
    media: {
        height: 0,
        paddingTop: '56.25%', // 16:9
    },
    expand: {
        marginLeft: 'auto',
        fontStyle: "italic"
    }
}));

export const DatasetOverview = ({ data, onMore }) => {

    const classes = useStyles();

    return (
        <Card className={classes.root}>
            <DatasetCardHeader data={data} />
            <CardContent>
                <Typography variant="body2" color="textPrimary" component="p">
                    {data.datasetDescription}
                </Typography>
            </CardContent>
            <CardActions disableSpacing>
                <IconButton aria-label="add to favorites">
                    <FavoriteIcon />
                </IconButton>
                <IconButton aria-label="share">
                    <ShareIcon />
                </IconButton>
                <Button 
                    className={classes.expand}
                    onClick={onMore}>
                    БІЛЬШЕ...
                </Button>
            </CardActions>
        </Card>
    );
};

export default DatasetOverview;