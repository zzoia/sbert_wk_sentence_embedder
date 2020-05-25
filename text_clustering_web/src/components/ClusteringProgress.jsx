import React from "react";
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import ListItemIcon from '@material-ui/core/ListItemIcon';
import ListItemText from '@material-ui/core/ListItemText';
import CheckRoundedIcon from '@material-ui/icons/CheckRounded';
import DoneAllRoundedIcon from '@material-ui/icons/DoneAllRounded';
import LinearProgress from '@material-ui/core/LinearProgress';

export const ClusteringProgress = () => {

    const generate = () => {
        return [
            { text: "Отримання векторів для кожного токена", icon: (<DoneAllRoundedIcon />) },
            { text: "Фільтрація токенів", icon: (<DoneAllRoundedIcon />) },
            { text: "Знаходження тем", icon: (<CheckRoundedIcon />) },
            { text: "Кластеризація тем", icon: (<CheckRoundedIcon />) }
        ].map(({ text, icon }, index) => (
            <ListItem key={index}>
                <ListItemIcon>
                    {icon}
                </ListItemIcon>
                <ListItemText primary={text} />
            </ListItem>
        ));
    };

    return (
        <React.Fragment>
            <LinearProgress color="secondary" />
            <List>{generate()}</List>
        </React.Fragment>
    );
};