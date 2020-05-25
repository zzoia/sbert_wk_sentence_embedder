import React, { useRef, useState } from 'react';
import Card from '@material-ui/core/Card';
import CardActions from '@material-ui/core/CardActions';
import CardContent from '@material-ui/core/CardContent';
import Button from '@material-ui/core/Button';
import Typography from '@material-ui/core/Typography';
import Snackbar from '@material-ui/core/Snackbar';
import Alert from '@material-ui/lab/Alert';
import TextField from '@material-ui/core/TextField';
import Box from '@material-ui/core/Box';

export const FilePicker = ({ onUploaded }) => {

    const fileNamePlaceholder = "Оберіть файл...";
    const maxSizeInBytes = 5000;
    const maxSizeErrorMessage = `Будь ласка, оберіть файл з розміром менше, ніж ${maxSizeInBytes} байт.`;

    const filePicker = useRef(null);

    const [fileName, setFileName] = useState(fileNamePlaceholder);
    const [isError, setIsError] = useState(false);

    const handleClose = (event, reason) => {
        if (reason === 'clickaway') return;
        setIsError(false);
    };

    const onFileChange = event => {
        
        const currentFile = event.target.files[0];        
        if (currentFile && currentFile.size > maxSizeInBytes) {
            setIsError(true);
            return;
        }

        setIsError(false);

        setFileName(currentFile ? currentFile.name : fileNamePlaceholder);
        onUploaded(currentFile);
    };

    return (
        <React.Fragment>
            <Card>
                <CardContent>
                    <Typography
                        gutterBottom
                        variant="h5"
                        component="h2">
                        Завантаження даних
                    </Typography>
                    <Typography
                        variant="body2"
                        color="textSecondary"
                        component="p">
                        Оберіть файл з *.json форматом, в якому міститься масив стрічок, кожна з яких є документом, який буде аналізуватись. Або оберіть файл *.txt, де кожен рядок є новим текстом.
                    </Typography>
                    <Box marginTop={2}>
                        <TextField
                            readOnly={true}
                            value={fileName}
                            label="Назва файлу"
                            variant="outlined"
                            fullWidth />
                    </Box>
                </CardContent>
                <CardActions>
                    <input
                        type="file"
                        onChange={onFileChange}
                        ref={filePicker}
                        style={{ display: "none" }}
                        accept=".json,.txt" />
                    <Button
                        size="large"
                        fullWidth
                        variant="contained"
                        color="primary"
                        onClick={() => filePicker.current.click()}
                    >
                        ОБРАТИ ФАЙЛ...
                </Button>
                </CardActions>
            </Card >
            <Snackbar open={isError} autoHideDuration={6000} onClose={handleClose}>
                <Alert onClose={handleClose} severity="error">
                    {maxSizeErrorMessage}
                </Alert>
            </Snackbar>
        </React.Fragment>
    );
};

export default FilePicker;