import React, { useState, useEffect } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Button from '@material-ui/core/Button';
import { useHistory } from "react-router-dom";
import DatasetOverview from "../components/DatasetOverview";
import Header from "../components/Header";
import Box from '@material-ui/core/Box';
import { getDatasetClusterings } from "../actions/apiActions";

const useStyles = makeStyles((theme) => ({
    root: {
        flexGrow: 1,
    },
    paper: {
        padding: theme.spacing(2),
        textAlign: 'center',
        color: theme.palette.text.secondary,
    },
    header: {
        backgroundColor: theme.palette.primary.main,
        height: 210,
        color: theme.palette.common.white,
        marginBottom: theme.spacing(2)
    },
    spaced: {
        marginBottom: theme.spacing(2)
    },
    datasets: {
        display: "flex",
        flexWrap: "wrap",
        justifyContent: "space-between",
        paddingLeft: theme.spacing(2),
        paddingRight: theme.spacing(2)
    }
}));

export const Dashboard = () => {

    const classes = useStyles();
    const history = useHistory();
    const [datasets, setDatasets] = useState([]);

    const fetchDatasets = async () => {
        const datasets = await getDatasetClusterings();
        setDatasets(datasets);
    };

    useEffect(() => fetchDatasets(), []);

    const onUpload = () => history.push("upload");
    const onDatasetMoreClicked = (id) => history.push(`datasets/${id}`);

    const datasetViews = datasets.map((dataset, idx) => 
        (<DatasetOverview
            key={idx} 
            onMore={() => onDatasetMoreClicked(dataset.id)}
            data={dataset} />));

    return (
        <div className={classes.root}>
            <Header headerText={"Активні набори даних"}>
                <Button
                    className={classes.spaced}
                    variant="contained"
                    color="secondary"
                    onClick={onUpload}
                >
                    ЗАВАНТАЖИТИ НОВИЙ
                </Button>
            </Header>

            <Box className={classes.datasets}>
                {datasetViews}
            </Box>
        </div>
    );
};

export default Dashboard;