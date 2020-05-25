import React, { useState } from 'react';
import { makeStyles } from '@material-ui/core/styles';
import Typography from '@material-ui/core/Typography';
import Button from '@material-ui/core/Button';
import Header from "../components/Header";
import Grid from '@material-ui/core/Grid';
import Paper from '@material-ui/core/Paper';
import { FilePicker } from '../components/FilePicker';
import { ClusteringProgress } from '../components/ClusteringProgress';
import { getNumberOfTexts, getTexts } from "../actions/fileActions"
import { cluster } from "../actions/apiActions";
import { useHistory } from "react-router-dom";

const useStyles = makeStyles((theme) => ({
    root: {
        flexGrow: 1,
    },
    paper: {
        display: "flex",
        flexDirection: "column",
        padding: theme.spacing(2),
        "& > * + *": {
            marginTop: theme.spacing(2),
        }
    },
    uploadContainer: {
        paddingLeft: theme.spacing(2),
        paddingRight: theme.spacing(2)
    }
}));

export const Upload = () => {

    const startText = "Почати кластеризацію";
    const stopText = "Скасувати";

    const history = useHistory();
    const classes = useStyles();

    const [file, setFile] = useState(null);
    const [numberOfTexts, setNumberOfTexts] = useState(0);
    const [showButton, setShowButton] = useState(false);
    const [buttonText, setButtonText] = useState(startText);
    const [isInProgress, setIsInProgress] = useState(false);
    const [clusterNumber, setClusterNumber] = useState(null);
    const [clusteringId, setClusteringId] = useState(0);

    const onFileChosen = file => {
        setFile(file);
        setShowButton(!!file);
        setClusterNumber(null);

        if (!file) return;

        getNumberOfTexts(file).then(setNumberOfTexts);
    };

    const toggleProgress = inProgress => {
        setIsInProgress(inProgress);
        setButtonText(inProgress ? stopText : startText);
    };

    const showAndReset = clustering => {
        toggleProgress(false);
        setClusterNumber(clustering.clusterCount);
        setClusteringId(clustering.id);
        setShowButton(false);
    };

    const onButtonClicked = async () => {
        const newValue = !isInProgress;
        toggleProgress(newValue);

        if (!newValue) return;

        const texts = await getTexts(file);
        const clustering = await cluster({
            texts: texts,
            datasetName: file.name.split(".").slice(0, -1).join("."),
            datasetDescription: "Опис датасету"
        });

        showAndReset(clustering);
    };

    const navigateToDetails = () => history.push(`datasets/${clusteringId}`);

    return (
        <div className={classes.root}>
            <Header headerText={"Завантажте набір даних"}>

            </Header>
            <Grid className={classes.uploadContainer} container spacing={3}>
                <Grid item xs={4}>
                    <FilePicker onUploaded={onFileChosen} />
                </Grid>
                <Grid item xs={8}>
                    <Paper className={classes.paper}>
                        <Typography>У вибраному файлі {numberOfTexts} текстів.</Typography>
                        {clusterNumber && (
                            <Button
                                variant="outlined"
                                onClick={navigateToDetails}
                            >
                                Знайдено {clusterNumber} кластерів
                            </Button>
                        )}
                        {showButton && (<Button
                            fullWidth
                            variant="contained"
                            color="secondary"
                            onClick={onButtonClicked}
                        >
                            {buttonText}
                        </Button>)}
                        {isInProgress && <ClusteringProgress />}
                    </Paper>
                </Grid>
            </Grid>
        </div>
    );
};

export default Upload;

