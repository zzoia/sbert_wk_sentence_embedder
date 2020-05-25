import React from "react";
import { makeStyles } from '@material-ui/core/styles';
import List from '@material-ui/core/List';
import ListItem from '@material-ui/core/ListItem';
import Paper from '@material-ui/core/Paper';
import ListItemText from '@material-ui/core/ListItemText';
import Divider from '@material-ui/core/Divider';
import OverflowedClusterTexts from "./OverflowedClusterTexts";
import TopicTokens from "./TopicTokens";

const useStyles = makeStyles((theme) => ({
    root: {
        width: "33%",
        minWidth: "400px",
        marginBottom: theme.spacing(2)
    },
    divider: {
        marginRight: theme.spacing(2),
        marginLeft: theme.spacing(2)
    }
}));

export const Cluster = ({ texts, topics, textCount }) => {

    const maxToDisplay = 3;
    const classes = useStyles();

    const textViews = texts.slice(0, maxToDisplay).map((text, idx) => (
        <React.Fragment key={idx}>
            <ListItem>
                <ListItemText primary={text} />
            </ListItem>
            {idx !== texts.length - 1 && <Divider className={classes.divider} />}
        </React.Fragment>
    ));

    const colors = ["#d94b41", "#d9a441", "#419673", "#1eb3c9", "#761ec9"];
    const shuffled = colors
        .map(a => ({ sort: Math.random(), value: a }))
        .sort((a, b) => a.sort - b.sort)
        .map(a => a.value);

    const topicViews = topics.map((topic, idx) => (
        <ListItem key={idx}>
            <TopicTokens
                tokens={topic.tokens}
                color={shuffled[idx % colors.length]}></TopicTokens>
        </ListItem>
    ));

    const overflowed = texts.slice(maxToDisplay);

    return (
        <Paper variant="outlined" className={classes.root}>
            <List>
                <ListItem>
                    <ListItemText primary={`${textCount} текстів`} />
                </ListItem>
                {topicViews}
                {textViews}
                <OverflowedClusterTexts texts={overflowed} />
            </List>
        </Paper >
    );
};

export default Cluster;