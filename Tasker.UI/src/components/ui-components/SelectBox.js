import React from 'react';

export default class SelectBox extends React.Component {
    constructor(props) {
      super(props);
      
      this.state = {
          taskId: props.taskId,
          currentValue: props.currentValue,
          values: props.values,
          action: props.action
        };
    }

    onChange(e) {
      this.setState({
        currentValue: e.target.value
      })

      this.state.action(this.state.taskId, e.target.value);
    }

    render() {
      return (
        <div className="form-group">
            <select 
                value={this.state.currentValue}
                onChange={this.onChange.bind(this)} className="form-control">
                    {this.state.values.map(option => {
                        return <option value={option} key={option}>{option}</option>
                    })}
            </select>
        </div>
      )
    }
  }