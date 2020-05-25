import React, { useEffect, useState } from "react";
import { makeStyles } from '@material-ui/core/styles';
import { withRouter } from 'react-router-dom';
import { getClusters, getDatasetClustering } from "../actions/apiActions";
import DatasetCardHeader from "../components/DatasetCardHeader";
import Cluster from "../components/Cluster";
import Header from "../components/Header";
import Card from '@material-ui/core/Card';
import CardContent from '@material-ui/core/CardContent';
import Box from '@material-ui/core/Box';
import CardActions from '@material-ui/core/CardActions';
import IconButton from '@material-ui/core/IconButton';
import FavoriteIcon from '@material-ui/icons/Favorite';
import ShareIcon from '@material-ui/icons/Share';

const useStyles = makeStyles((theme) => ({
    root: {
        flexGrow: 1
    },
    card: {
        margin: theme.spacing(2)
    },
    clusters: {
        display: "flex",
        flexWrap: "wrap",
        alignItems: "flex-start",
        justifyContent: "space-between"
    }
}));

export const DatasetDetails = ({ match }) => {

    const classes = useStyles();

    const [datasetClustering, setDatasetClustering] = useState(null);
    const [clusters, setClusters] = useState([]);

    useEffect(() => {
        const datasetClusteringId = +match.params.id;

        getDatasetClustering(datasetClusteringId).then(setDatasetClustering);
        getClusters(datasetClusteringId).then(setClusters);

    }, [match]);

    if (!datasetClustering) return null;

    const clusterViews = clusters.map(({ texts, topics, textCount }, idx) => (<Cluster texts={texts} topics={topics} textCount={textCount} key={idx} />));

    return (
        <div className={classes.root}>
            <Header headerText={datasetClustering.datasetName} />
            <Card className={classes.card}>
                <DatasetCardHeader data={datasetClustering} />
                <CardContent>
                    <Box className={classes.clusters}>
                        {clusterViews}
                    </Box>
                </CardContent>
                <CardActions disableSpacing>
                    <IconButton aria-label="add to favorites">
                        <FavoriteIcon />
                    </IconButton>
                    <IconButton aria-label="share">
                        <ShareIcon />
                    </IconButton>
                </CardActions>
            </Card>
        </div>
    );

};

export default withRouter(DatasetDetails);